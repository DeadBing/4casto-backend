namespace FourCasto.Domain.Markets;

using FourCasto.Contracts.Enums;

public class Market
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid? SignalId { get; set; }
    public Guid SubjectId { get; set; }
    public MarketType MarketType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MarketStatus Status { get; set; } = MarketStatus.DRAFT;
    public DateTime? OpensAt { get; set; }
    public DateTime? ClosesAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Signal? Signal { get; set; }
    public Subject Subject { get; set; } = null!;
}
