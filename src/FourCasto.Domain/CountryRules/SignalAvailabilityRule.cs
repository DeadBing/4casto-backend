namespace FourCasto.Domain.CountryRules;

public class SignalAvailabilityRule
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid? SubjectGroupId { get; set; }
    public decimal MaxProgressPercent { get; set; } = 90m;
    public bool RequireFreshPrice { get; set; } = true;
    public int MaxPriceStalenessSeconds { get; set; } = 60;
    public Guid ConfigVersionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ConfigVersion ConfigVersion { get; set; } = null!;
}
