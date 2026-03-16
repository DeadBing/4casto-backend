namespace FourCasto.Domain.Settlement;

using FourCasto.Contracts.Enums;

public class BetCancellationExecution
{
    public Guid Id { get; set; }
    public Guid CancellationRequestId { get; set; }
    public Guid BetId { get; set; }
    public decimal PenaltyPercent { get; set; }
    public decimal PenaltyAmount { get; set; }
    public decimal AmountReturned { get; set; }
    public decimal HoldReleaseAmount { get; set; }
    public Guid? LedgerEntryId { get; set; }
    public CancellationExecutionStatus Status { get; set; } = CancellationExecutionStatus.PENDING;
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public string? ErrorMessage { get; set; }

    public BetCancellationRequest CancellationRequest { get; set; } = null!;
}
