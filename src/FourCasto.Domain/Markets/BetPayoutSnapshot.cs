namespace FourCasto.Domain.Markets;

using FourCasto.Contracts.Enums;

public class BetPayoutSnapshot
{
    public Guid Id { get; set; }
    public Guid BetId { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public Guid? CountryStatusId { get; set; }
    public Guid? SubjectGroupId { get; set; }
    public Guid? SubjectId { get; set; }
    public BetDirection BetDirection { get; set; }
    public decimal StakeAmount { get; set; }
    public decimal BasePayoutPercent { get; set; }
    public decimal? MarketPriceAtEntry { get; set; }
    public decimal? ProgressPercentAtEntry { get; set; }
    public ProgressDirection? ProgressDirectionAtEntry { get; set; }
    public decimal? ProgressAdjustmentPercent { get; set; }
    public string? ProgressAdjustmentReason { get; set; }
    public decimal FinalPayoutPercentApplied { get; set; }
    public decimal PotentialProfit { get; set; }
    public decimal TotalReturn { get; set; }
    public PolicySourceType? PolicySourceType { get; set; }
    public Guid? PolicySourceId { get; set; }
    public Guid? ConfigVersionId { get; set; }
    public DateTime QuotedAt { get; set; } = DateTime.UtcNow;
    public DateTime ConfirmedAt { get; set; } = DateTime.UtcNow;

    public Bet Bet { get; set; } = null!;
}
