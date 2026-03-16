namespace FourCasto.Domain.Settlement;

using FourCasto.Contracts.Enums;

public class BetPayout
{
    public Guid Id { get; set; }
    public Guid BetId { get; set; }
    public Guid SettlementId { get; set; }
    public decimal PayoutAmount { get; set; }
    public decimal PnlAmount { get; set; }
    public PayoutType PayoutType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Settlement Settlement { get; set; } = null!;
}
