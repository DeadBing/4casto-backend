namespace FourCasto.Application.Services;

using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Infrastructure.Persistence;
using FourCasto.Domain.CountryRules;

public class PolicyResolutionService : IPolicyResolutionService
{
    private readonly FourCastoDbContext _db;

    public PolicyResolutionService(FourCastoDbContext db)
    {
        _db = db;
    }

    public async Task<ResolvedPayoutPolicy> ResolvePayoutPolicyAsync(
        Guid fourCastoWlId,
        BetDirection direction,
        string? countryCode,
        Guid? countryStatusId,
        Guid? subjectGroupId,
        Guid? subjectId)
    {
        var rules = await _db.PayoutPolicyRules
            .Where(r => r.FourCastoWlId == fourCastoWlId)
            .Where(r => r.BetDirection == null || r.BetDirection == direction)
            .OrderBy(r => r.Priority)
            .ToListAsync();

        var chain = new List<string>();

        // 1. Subject override
        var subjectRule = rules.FirstOrDefault(r =>
            r.SubjectId == subjectId && r.SubjectId != null);
        if (subjectRule != null)
        {
            chain.Add($"SubjectOverride:{subjectRule.Id}");
            return new ResolvedPayoutPolicy(
                subjectRule.PayoutPercent,
                PolicySourceType.SUBJECT_OVERRIDE,
                subjectRule.Id,
                string.Join(" -> ", chain));
        }
        chain.Add("SubjectOverride:miss");

        // 2. SubjectGroup rule
        var groupRule = rules.FirstOrDefault(r =>
            r.SubjectGroupId == subjectGroupId && r.SubjectGroupId != null && r.SubjectId == null);
        if (groupRule != null)
        {
            chain.Add($"SubjectGroup:{groupRule.Id}");
            return new ResolvedPayoutPolicy(
                groupRule.PayoutPercent,
                PolicySourceType.SUBJECT_GROUP,
                groupRule.Id,
                string.Join(" -> ", chain));
        }
        chain.Add("SubjectGroup:miss");

        // 3. Country + CountryStatus rule
        var countryRule = rules.FirstOrDefault(r =>
            r.CountryCode == countryCode && r.CountryStatusId == countryStatusId
            && r.SubjectGroupId == null && r.SubjectId == null);
        if (countryRule != null)
        {
            chain.Add($"CountryStatus:{countryRule.Id}");
            return new ResolvedPayoutPolicy(
                countryRule.PayoutPercent,
                PolicySourceType.COUNTRY_STATUS,
                countryRule.Id,
                string.Join(" -> ", chain));
        }
        chain.Add("CountryStatus:miss");

        // 4. WL default
        var wlDefault = rules.FirstOrDefault(r =>
            r.CountryCode == null && r.CountryStatusId == null
            && r.SubjectGroupId == null && r.SubjectId == null);
        if (wlDefault != null)
        {
            chain.Add($"WlDefault:{wlDefault.Id}");
            return new ResolvedPayoutPolicy(
                wlDefault.PayoutPercent,
                PolicySourceType.WL_DEFAULT,
                wlDefault.Id,
                string.Join(" -> ", chain));
        }
        chain.Add("WlDefault:miss");

        // Fallback hardcoded
        chain.Add("Hardcoded:80%");
        return new ResolvedPayoutPolicy(80m, PolicySourceType.WL_DEFAULT, null, string.Join(" -> ", chain));
    }

    public async Task<ResolvedCancellationPolicy> ResolveCancellationPolicyAsync(
        Guid fourCastoWlId,
        string? countryCode,
        Guid? countryStatusId,
        Guid? subjectGroupId,
        Guid? subjectId)
    {
        var policies = await _db.BetCancellationPolicies
            .Where(p => p.FourCastoWlId == fourCastoWlId)
            .ToListAsync();

        // Priority: Subject > SubjectGroup > Country+Status > WL default
        var match = policies.FirstOrDefault(p => p.SubjectId == subjectId && p.SubjectId != null)
            ?? policies.FirstOrDefault(p => p.SubjectGroupId == subjectGroupId && p.SubjectGroupId != null && p.SubjectId == null)
            ?? policies.FirstOrDefault(p => p.CountryCode == countryCode && p.CountryStatusId == countryStatusId && p.SubjectGroupId == null)
            ?? policies.FirstOrDefault(p => p.CountryCode == null && p.CountryStatusId == null && p.SubjectGroupId == null && p.SubjectId == null);

        if (match != null)
            return new ResolvedCancellationPolicy(match.IsAllowed, match.PenaltyPercent, match.Id);

        // Default: allowed with 30% penalty
        return new ResolvedCancellationPolicy(true, 30m, null);
    }
}
