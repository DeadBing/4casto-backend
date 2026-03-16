namespace FourCasto.Domain.Settlement;

public class SettlementAdjustment
{
    public Guid Id { get; set; }
    public Guid SettlementId { get; set; }
    public Guid BetId { get; set; }
    public decimal OriginalPayoutAmount { get; set; }
    public decimal AdjustedPayoutAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string AdjustedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Settlement Settlement { get; set; } = null!;
}
