namespace FourCasto.Domain.Settlement;

using FourCasto.Contracts.Enums;

public class BetCancellationRequest
{
    public Guid Id { get; set; }
    public Guid BetId { get; set; }
    public Guid UserId { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public CancellationRequestStatus Status { get; set; } = CancellationRequestStatus.PENDING;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}
