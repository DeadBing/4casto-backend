namespace FourCasto.Domain.Admin;

using FourCasto.Contracts.Enums;

public class IdempotencyRecord
{
    public Guid Id { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public IdempotencyStatus Status { get; set; } = IdempotencyStatus.PROCESSING;
    public string? ResultPayload { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}
