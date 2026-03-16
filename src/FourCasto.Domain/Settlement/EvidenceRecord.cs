namespace FourCasto.Domain.Settlement;

public class EvidenceRecord
{
    public Guid Id { get; set; }
    public string RefType { get; set; } = string.Empty;
    public Guid RefId { get; set; }
    public string EvidenceType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
