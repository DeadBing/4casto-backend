namespace FourCasto.Domain.Fraud;

public class FraudRule
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string RuleType { get; set; } = string.Empty;
    public string? Config { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
