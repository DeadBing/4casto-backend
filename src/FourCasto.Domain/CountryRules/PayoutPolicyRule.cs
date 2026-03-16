namespace FourCasto.Domain.CountryRules;

using FourCasto.Contracts.Enums;

public class PayoutPolicyRule
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public string? CountryCode { get; set; }
    public Guid? CountryStatusId { get; set; }
    public Guid? SubjectGroupId { get; set; }
    public Guid? SubjectId { get; set; }
    public BetDirection? BetDirection { get; set; }
    public decimal PayoutPercent { get; set; }
    public int Priority { get; set; }
    public Guid ConfigVersionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ConfigVersion ConfigVersion { get; set; } = null!;
}
