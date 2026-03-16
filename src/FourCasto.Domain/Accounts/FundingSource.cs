namespace FourCasto.Domain.Accounts;

public class FundingSource
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public string SourceType { get; set; } = string.Empty; // BANK_CARD, CRYPTO_WALLET, etc.
    public string? DisplayName { get; set; }
    public string? MaskedDetails { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
