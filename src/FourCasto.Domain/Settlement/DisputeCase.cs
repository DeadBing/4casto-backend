namespace FourCasto.Domain.Settlement;

using FourCasto.Contracts.Enums;

public class DisputeCase
{
    public Guid Id { get; set; }
    public string RefType { get; set; } = string.Empty;
    public Guid RefId { get; set; }
    public Guid UserId { get; set; }
    public DisputeStatus Status { get; set; } = DisputeStatus.OPEN;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    public ICollection<DisputeComment> Comments { get; set; } = new List<DisputeComment>();
}
