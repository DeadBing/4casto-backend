namespace FourCasto.Domain.CountryRules;

public class ConfigVersion
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public int VersionNumber { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
}
