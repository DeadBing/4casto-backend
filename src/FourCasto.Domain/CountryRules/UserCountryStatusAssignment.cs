namespace FourCasto.Domain.CountryRules;

public class UserCountryStatusAssignment
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public Guid CountryStatusId { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ValidUntil { get; set; }

    public CountryStatus CountryStatus { get; set; } = null!;
}
