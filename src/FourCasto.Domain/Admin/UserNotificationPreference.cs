namespace FourCasto.Domain.Admin;

using FourCasto.Contracts.Enums;

public class UserNotificationPreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationEventType EventType { get; set; }
    public NotificationChannel Channel { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
