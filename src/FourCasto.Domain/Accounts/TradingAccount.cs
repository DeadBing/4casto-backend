namespace FourCasto.Domain.Accounts;

using FourCasto.Contracts.Enums;
using FourCasto.Domain.Identity;

public class TradingAccount
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public AccountType AccountType { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "USD";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public FourCastoWl FourCastoWl { get; set; } = null!;
    public User User { get; set; } = null!;
    public TradingAccountBalance? Balance { get; set; }
}
