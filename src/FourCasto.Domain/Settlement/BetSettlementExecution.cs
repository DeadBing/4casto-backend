namespace FourCasto.Domain.Settlement;

using FourCasto.Contracts.Enums;

public class BetSettlementExecution
{
    public Guid Id { get; set; }
    public Guid BetId { get; set; }
    public Guid SettlementId { get; set; }
    public SettlementExecutionStatus Status { get; set; } = SettlementExecutionStatus.PENDING;
    public int AttemptNumber { get; set; } = 1;
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }

    public Settlement Settlement { get; set; } = null!;
}
