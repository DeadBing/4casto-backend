namespace FourCasto.Domain.Accounts;

using FourCasto.Contracts.Enums;

public class LedgerEntry
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public LedgerAccountType AccountType { get; set; }
    public Guid AccountId { get; set; }
    public BalanceType BalanceType { get; set; }
    public LedgerEntryType EntryType { get; set; }
    public decimal Amount { get; set; }
    public string? RefType { get; set; }
    public Guid? RefId { get; set; }
    public decimal BalanceTotalAfter { get; set; }
    public decimal BalanceAvailableAfter { get; set; }
    public decimal BalanceLockedAfter { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
