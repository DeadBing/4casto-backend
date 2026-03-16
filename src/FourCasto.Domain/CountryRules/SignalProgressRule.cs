namespace FourCasto.Domain.CountryRules;

using FourCasto.Contracts.Enums;

public class SignalProgressRule
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid? SubjectGroupId { get; set; }
    public decimal ProgressFrom { get; set; }
    public decimal ProgressTo { get; set; }
    public ProgressDirection ProgressDirection { get; set; }
    public decimal PayoutAdjustmentPercent { get; set; }
    public bool IsBettingAllowed { get; set; } = true;
    public Guid ConfigVersionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ConfigVersion ConfigVersion { get; set; } = null!;
}
