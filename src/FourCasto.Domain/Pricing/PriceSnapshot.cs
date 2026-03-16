namespace FourCasto.Domain.Pricing;

public class PriceSnapshot
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? QuoteSourceId { get; set; }
    public decimal Price { get; set; }
    public DateTime SnapshotAt { get; set; } = DateTime.UtcNow;
}
