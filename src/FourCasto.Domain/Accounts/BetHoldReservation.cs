namespace FourCasto.Domain.Accounts;

using FourCasto.Contracts.Enums;

public class BetHoldReservation
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public Guid TradingAccountId { get; set; }
    public Guid BetId { get; set; }
    public decimal AmountLocked { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public HoldStatus Status { get; set; } = HoldStatus.ACTIVE;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReleasedAt { get; set; }
    public HoldReleaseReason? ReleaseReason { get; set; }

    // Navigation
    public TradingAccount TradingAccount { get; set; } = null!;
}
