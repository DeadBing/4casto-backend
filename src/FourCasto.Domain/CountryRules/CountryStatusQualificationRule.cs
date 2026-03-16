namespace FourCasto.Domain.CountryRules;

using FourCasto.Contracts.Enums;

public class CountryStatusQualificationRule
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public Guid CountryStatusId { get; set; }
    public QualificationMetricType MetricType { get; set; }
    public decimal MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public Guid ConfigVersionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CountryStatus CountryStatus { get; set; } = null!;
    public ConfigVersion ConfigVersion { get; set; } = null!;
}
