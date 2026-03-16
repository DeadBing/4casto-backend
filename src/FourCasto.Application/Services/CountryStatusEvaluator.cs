namespace FourCasto.Application.Services;

using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
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
                    && s.Name == Contracts.Enums.CountryStatusName.STANDARD);

            return new CountryStatusResult(standardStatus?.Id, "STANDARD", countryCode);
        }

        // Calculate user's qualifying metric (total balance across real trading accounts)
        var totalBalance = await _db.TradingAccountBalances
            .Where(b => _db.TradingAccounts
                .Any(ta => ta.Id == b.TradingAccountId
                    && ta.UserId == userId
                    && ta.FourCastoWlId == fourCastoWlId
                    && ta.AccountType == Contracts.Enums.AccountType.REAL))
            .SumAsync(b => b.TotalBalance);

        // Find matching rule
        foreach (var rule in rules)
        {
            if (totalBalance >= rule.MinValue && (rule.MaxValue == null || totalBalance <= rule.MaxValue))
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
