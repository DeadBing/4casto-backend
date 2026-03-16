namespace FourCasto.Domain.CountryRules;

public class BetCancellationPolicy
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public string? CountryCode { get; set; }
    public Guid? CountryStatusId { get; set; }
    public Guid? SubjectGroupId { get; set; }
    public Guid? SubjectId { get; set; }
    public bool IsAllowed { get; set; } = true;
    public decimal PenaltyPercent { get; set; }
    public Guid ConfigVersionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ConfigVersion ConfigVersion { get; set; } = null!;
}
