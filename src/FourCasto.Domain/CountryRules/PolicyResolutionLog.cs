namespace FourCasto.Domain.CountryRules;

public class PolicyResolutionLog
{
    public Guid Id { get; set; }
    public Guid BetId { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public Guid? ResolvedPolicyId { get; set; }
    public string? FallbackChain { get; set; }
    public DateTime ResolvedAt { get; set; } = DateTime.UtcNow;
}
