namespace FourCasto.Domain.Settlement;

using FourCasto.Contracts.Enums;

public class Settlement
{
    public Guid Id { get; set; }
    public Guid MarketId { get; set; }
    public Guid? SignalOutcomeId { get; set; }
    public SettlementStatus Status { get; set; } = SettlementStatus.PENDING;
    public string? SettledBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}
