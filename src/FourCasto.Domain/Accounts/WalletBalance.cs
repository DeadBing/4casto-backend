namespace FourCasto.Domain.Accounts;

public class WalletBalance
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal LockedBalance { get; set; }
    public decimal WithdrawableBalance { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public uint RowVersion { get; set; }

    // Navigation
    public Wallet Wallet { get; set; } = null!;
}
