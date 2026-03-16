namespace FourCasto.Domain.Pricing;

public class PriceTick
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public Guid? QuoteSourceId { get; set; }
    public decimal Price { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}
