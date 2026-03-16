namespace FourCasto.Domain.Markets;

using FourCasto.Contracts.Enums;

public class Bet
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public Guid TradingAccountId { get; set; }
    public Guid MarketId { get; set; }
    public Guid? SignalOutcomeId { get; set; }
    public BetDirection Direction { get; set; }
    public decimal StakeAmount { get; set; }
    public BetStatus Status { get; set; } = BetStatus.OPEN;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SettledAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public Market Market { get; set; } = null!;
    public BetPayoutSnapshot? PayoutSnapshot { get; set; }
    public BetEntryPriceContext? EntryPriceContext { get; set; }
}
