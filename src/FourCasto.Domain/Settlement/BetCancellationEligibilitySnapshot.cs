namespace FourCasto.Domain.Settlement;

using FourCasto.Contracts.Enums;

public class BetCancellationEligibilitySnapshot
{
    public Guid Id { get; set; }
    public Guid BetId { get; set; }
    public decimal CurrentMarketPrice { get; set; }
    public decimal EntryPrice { get; set; }
    public BetDirection BetDirection { get; set; }
    public SignalDirection SignalDirection { get; set; }
    public bool IsInPositiveZone { get; set; }
    public bool IsCancellationAllowed { get; set; }
    public decimal? ApplicablePenaltyPercent { get; set; }
    public string? DenialReason { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}
