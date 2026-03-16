namespace FourCasto.Domain.Markets;

using FourCasto.Contracts.Enums;

public class SignalLifecycleSnapshot
{
    public Guid Id { get; set; }
    public Guid SignalId { get; set; }
    public decimal CurrentMarketPrice { get; set; }
    public decimal ProgressPercent { get; set; }
    public ProgressDirection ProgressDirection { get; set; }
    public bool IsAvailableForBetting { get; set; }
    public DateTime SnapshotAt { get; set; } = DateTime.UtcNow;

    public Signal Signal { get; set; } = null!;
}
