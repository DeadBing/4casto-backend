namespace FourCasto.Domain.Identity;

using FourCasto.Contracts.Enums;

public class User
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public AuthProvider AuthProvider { get; set; } = AuthProvider.EMAIL;
    public bool IsGuest { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public FourCastoWl FourCastoWl { get; set; } = null!;
    public UserProfile? Profile { get; set; }
}
