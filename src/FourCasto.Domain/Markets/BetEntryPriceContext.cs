namespace FourCasto.Domain.Markets;

using FourCasto.Contracts.Enums;

public class BetEntryPriceContext
{
    public Guid Id { get; set; }
    public Guid BetId { get; set; }
    public Guid SignalId { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal CurrentMarketPrice { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal StopLossPrice { get; set; }
    public decimal ProgressPercent { get; set; }
    public ProgressDirection ProgressDirection { get; set; }
    public DateTime SnapshotAt { get; set; } = DateTime.UtcNow;

    public Bet Bet { get; set; } = null!;
}
