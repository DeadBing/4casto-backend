namespace FourCasto.Domain.Fraud;

using FourCasto.Contracts.Enums;

public class UserRestriction
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public RestrictionType RestrictionType { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}
