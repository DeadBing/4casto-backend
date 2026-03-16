namespace FourCasto.Domain.Fraud;

using FourCasto.Contracts.Enums;

public class FraudEvent
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid UserId { get; set; }
    public Guid? FraudRuleId { get; set; }
    public FraudEventType EventType { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public FraudRule? FraudRule { get; set; }
}
