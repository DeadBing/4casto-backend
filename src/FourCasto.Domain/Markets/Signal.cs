namespace FourCasto.Domain.Markets;

using FourCasto.Contracts.Enums;

public class Signal
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid SubjectId { get; set; }
    public SignalType SignalType { get; set; }
    public SignalDirection SignalDirection { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal StopLossPrice { get; set; }
    public decimal MaxBettingProgressPercent { get; set; } = 90m;
    public decimal BasePayoutPercentAgree { get; set; }
    public decimal BasePayoutPercentDisagree { get; set; }
    public SignalStatus Status { get; set; } = SignalStatus.ACTIVE;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Subject Subject { get; set; } = null!;
    public SignalOutcome? Outcome { get; set; }
}
