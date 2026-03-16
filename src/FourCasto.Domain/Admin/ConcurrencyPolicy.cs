namespace FourCasto.Domain.Admin;

using FourCasto.Contracts.Enums;

public class ConcurrencyPolicy
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public LockingStrategy LockingStrategy { get; set; }
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 100;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
