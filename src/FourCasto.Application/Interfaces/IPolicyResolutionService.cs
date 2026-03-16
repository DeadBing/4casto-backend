namespace FourCasto.Application.Interfaces;

using FourCasto.Contracts.Enums;

public record ResolvedPayoutPolicy(
    decimal PayoutPercent,
    PolicySourceType SourceType,
    Guid? PolicyId,
    string FallbackChain
);

public record ResolvedCancellationPolicy(
    bool IsAllowed,
    decimal PenaltyPercent,
    Guid? PolicyId
);

public interface IPolicyResolutionService
{
    Task<ResolvedPayoutPolicy> ResolvePayoutPolicyAsync(
        Guid fourCastoWlId,
        BetDirection direction,
        string? countryCode,
        Guid? countryStatusId,
        Guid? subjectGroupId,
        Guid? subjectId);

    Task<ResolvedCancellationPolicy> ResolveCancellationPolicyAsync(
        Guid fourCastoWlId,
        string? countryCode,
        Guid? countryStatusId,
        Guid? subjectGroupId,
        Guid? subjectId);
}
