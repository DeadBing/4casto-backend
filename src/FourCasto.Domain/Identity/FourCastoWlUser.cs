namespace FourCasto.Domain.Identity;

using FourCasto.Contracts.Enums;

public class FourCastoWlUser
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public UserRole Role { get; set; } = UserRole.USER;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public FourCastoWl FourCastoWl { get; set; } = null!;
    public User User { get; set; } = null!;
}
