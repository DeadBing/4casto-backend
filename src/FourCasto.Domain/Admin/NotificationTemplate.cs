namespace FourCasto.Domain.Admin;

using FourCasto.Contracts.Enums;

public class NotificationTemplate
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public NotificationEventType EventType { get; set; }
    public NotificationChannel Channel { get; set; }
    public string TemplateBody { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
