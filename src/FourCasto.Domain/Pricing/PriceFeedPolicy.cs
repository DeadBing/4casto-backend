namespace FourCasto.Domain.Pricing;

public class PriceFeedPolicy
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid QuoteSourceId { get; set; }
    public int MaxStalenessSeconds { get; set; } = 60;
    public bool AllowBetWithoutFreshPrice { get; set; }
    public bool AllowSettlementWithoutFreshPrice { get; set; }
    public bool AllowCancellationWithoutFreshPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public QuoteSource QuoteSource { get; set; } = null!;
}
