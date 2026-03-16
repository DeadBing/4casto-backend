namespace FourCasto.Domain.Markets;

using FourCasto.Contracts.Enums;

public class SignalProgressMetric
{
    public Guid Id { get; set; }
    public Guid SignalId { get; set; }
    public decimal ProgressPercent { get; set; }
    public ProgressDirection ProgressDirection { get; set; }
    public decimal MarketPrice { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    public Signal Signal { get; set; } = null!;
}
