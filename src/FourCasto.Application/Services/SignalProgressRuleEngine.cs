namespace FourCasto.Application.Services;

using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Infrastructure.Persistence;

public class SignalProgressRuleEngine
{
    private readonly FourCastoDbContext _db;

    public SignalProgressRuleEngine(FourCastoDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Look up payout adjustment from SignalProgressRules.
    /// Falls back to 0 if no rule matches.
    /// </summary>
    public async Task<decimal> GetPayoutAdjustmentAsync(
        Guid fourCastoWlId,
        Guid? subjectGroupId,
        decimal progressPercent,
        ProgressDirection direction)
    {
        var absProgress = Math.Abs(progressPercent);

        var rule = await _db.SignalProgressRules
            .Where(r => r.FourCastoWlId == fourCastoWlId)
            .Where(r => r.SubjectGroupId == null || r.SubjectGroupId == subjectGroupId)
            .Where(r => r.ProgressDirection == direction)
            .Where(r => absProgress >= r.ProgressFrom && absProgress < r.ProgressTo)
            .OrderByDescending(r => r.SubjectGroupId != null) // prefer specific over wildcard
            .FirstOrDefaultAsync();

        return rule?.PayoutAdjustmentPercent ?? 0m;
    }

    /// <summary>
    /// Check if betting is allowed based on SignalAvailabilityRules.
    /// Falls back to checking maxBettingProgressPercent from Signal if no rule exists.
    /// </summary>
    public async Task<bool> IsBettingAllowedAsync(
        Guid fourCastoWlId,
        Guid? subjectGroupId,
        decimal progressPercent)
    {
        var absProgress = Math.Abs(progressPercent);

        var rule = await _db.SignalAvailabilityRules
            .Where(r => r.FourCastoWlId == fourCastoWlId)
            .Where(r => r.SubjectGroupId == null || r.SubjectGroupId == subjectGroupId)
            .OrderByDescending(r => r.SubjectGroupId != null)
            .FirstOrDefaultAsync();

        if (rule != null)
            return absProgress <= rule.MaxProgressPercent;

        // No rule — use default 90%
        return absProgress <= 90m;
    }
}
