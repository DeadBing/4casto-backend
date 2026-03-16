namespace FourCasto.Domain.Accounts;

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public Guid? FundingSourceId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // DEPOSIT, WITHDRAWAL
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string Status { get; set; } = "PENDING";
    public string? ExternalTransactionId { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
