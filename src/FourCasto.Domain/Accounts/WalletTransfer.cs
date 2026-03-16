namespace FourCasto.Domain.Accounts;

using FourCasto.Contracts.Enums;

public class WalletTransfer
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public Guid TradingAccountId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public TransferDirection Direction { get; set; }
    public TransferStatus Status { get; set; } = TransferStatus.PENDING;
    public string IdempotencyKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }

    // Navigation
    public Wallet Wallet { get; set; } = null!;
    public TradingAccount TradingAccount { get; set; } = null!;
}
