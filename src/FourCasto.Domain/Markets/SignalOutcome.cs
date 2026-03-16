namespace FourCasto.Domain.Markets;

using FourCasto.Contracts.Enums;

public class SignalOutcome
{
    public Guid Id { get; set; }
    public Guid SignalId { get; set; }
    public SignalOutcomeType OutcomeType { get; set; }
    public decimal? ResolvedPrice { get; set; }
    public Guid? ResolutionSourceId { get; set; }
    public DateTime ResolvedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public Signal Signal { get; set; } = null!;
}
