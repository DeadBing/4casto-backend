namespace FourCasto.Domain.Accounts;

using FourCasto.Domain.Identity;

public class Wallet
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public FourCastoWl FourCastoWl { get; set; } = null!;
    public User User { get; set; } = null!;
    public WalletBalance? Balance { get; set; }
}
