namespace FourCasto.Domain.CountryRules;

public class RulePriorityPolicy
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public string PriorityOrder { get; set; } = string.Empty; // JSON array of priority levels
    public Guid ConfigVersionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ConfigVersion ConfigVersion { get; set; } = null!;
}
