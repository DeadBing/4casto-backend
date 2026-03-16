namespace FourCasto.Domain.Settlement;

public class ReasonCorrection
{
    public Guid Id { get; set; }
    public string RefType { get; set; } = string.Empty;
    public Guid RefId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string CorrectedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
