namespace FourCasto.Application.Services;

using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Infrastructure.Persistence;

public class CountryStatusEvaluator : ICountryStatusEvaluator
{
    private readonly FourCastoDbContext _db;

    public CountryStatusEvaluator(FourCastoDbContext db)
    {
        _db = db;
    }

    public async Task<CountryStatusResult> EvaluateAsync(Guid fourCastoWlId, Guid userId)
    {
        // Check cached assignment first
        var assignment = await _db.UserCountryStatusAssignments
            .Include(a => a.CountryStatus)
            .Where(a => a.FourCastoWlId == fourCastoWlId && a.UserId == userId)
            .Where(a => a.ValidUntil == null || a.ValidUntil > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (assignment != null)
        {
            return new CountryStatusResult(
                assignment.CountryStatusId,
                assignment.CountryStatus.Name.ToString(),
                assignment.CountryCode);
        }

        // Get user profile for country
        var profile = await _db.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        var countryCode = profile?.CountryCode ?? "US";

        // Get qualification rules for this country
        var rules = await _db.CountryStatusQualificationRules
            .Include(r => r.CountryStatus)
            .Where(r => r.FourCastoWlId == fourCastoWlId && r.CountryCode == countryCode)
            .OrderByDescending(r => r.MinValue)
            .ToListAsync();

        if (!rules.Any())
        {
            // Default to STANDARD
            var standardStatus = await _db.CountryStatuses
                .FirstOrDefaultAsync(s => s.FourCastoWlId == fourCastoWlId
                    && s.Name == CountryStatusName.STANDARD);

            return new CountryStatusResult(standardStatus?.Id, "STANDARD", countryCode);
        }

        // Group rules by metric type to minimize queries
        var metricTypes = rules.Select(r => r.MetricType).Distinct().ToList();

        var metrics = new Dictionary<QualificationMetricType, decimal>();

        foreach (var metricType in metricTypes)
        {
            metrics[metricType] = metricType switch
            {
                QualificationMetricType.CURRENT_BALANCE => await _db.TradingAccountBalances
                    .Where(b => _db.TradingAccounts
                        .Any(ta => ta.Id == b.TradingAccountId
                            && ta.UserId == userId
                            && ta.FourCastoWlId == fourCastoWlId
                            && ta.AccountType == AccountType.REAL))
                    .SumAsync(b => b.TotalBalance),

                QualificationMetricType.EQUITY => await _db.TradingAccountBalances
                    .Where(b => _db.TradingAccounts
                        .Any(ta => ta.Id == b.TradingAccountId
                            && ta.UserId == userId
                            && ta.FourCastoWlId == fourCastoWlId
                            && ta.AccountType == AccountType.REAL))
                    .SumAsync(b => b.Equity),

                QualificationMetricType.DEPOSIT_SUM => await _db.LedgerEntries
                    .Where(e => e.FourCastoWlId == fourCastoWlId
                        && e.EntryType == LedgerEntryType.DEPOSIT
                        && e.AccountType == LedgerAccountType.WALLET)
                    .Where(e => _db.Wallets
                        .Any(w => w.Id == e.AccountId && w.UserId == userId))
                    .SumAsync(e => e.Amount),

                QualificationMetricType.TRADING_VOLUME => await _db.Bets
                    .Where(b => b.FourCastoWlId == fourCastoWlId
                        && b.UserId == userId
                        && b.Status == BetStatus.SETTLED)
                    .SumAsync(b => b.StakeAmount),

                _ => 0m
            };
        }

        // Find matching rule
        foreach (var rule in rules)
        {
            var metricValue = metrics.GetValueOrDefault(rule.MetricType, 0m);
            if (metricValue >= rule.MinValue && (rule.MaxValue == null || metricValue <= rule.MaxValue))
            {
                return new CountryStatusResult(
                    rule.CountryStatusId,
                    rule.CountryStatus.Name.ToString(),
                    countryCode);
            }
        }

        // Fallback
        var fallbackStatus = rules.LastOrDefault();
        return new CountryStatusResult(
            fallbackStatus?.CountryStatusId,
            fallbackStatus?.CountryStatus?.Name.ToString() ?? "STANDARD",
            countryCode);
    }
}
