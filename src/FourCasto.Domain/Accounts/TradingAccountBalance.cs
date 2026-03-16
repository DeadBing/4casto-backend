namespace FourCasto.Domain.Accounts;

public class TradingAccountBalance
{
    public Guid Id { get; set; }
    public Guid TradingAccountId { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal LockedBalance { get; set; }
    public decimal BonusCredit { get; set; }
    public decimal Equity { get; set; }
    public decimal WithdrawableBalance { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public uint RowVersion { get; set; }

    // Navigation
    public TradingAccount TradingAccount { get; set; } = null!;
}
