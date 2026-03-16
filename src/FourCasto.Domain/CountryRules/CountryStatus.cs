namespace FourCasto.Domain.CountryRules;

using FourCasto.Contracts.Enums;

public class CountryStatus
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public CountryStatusName Name { get; set; }
    public int Rank { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
