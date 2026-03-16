namespace FourCasto.Domain.Admin;

using FourCasto.Contracts.Enums;

public class AdminOverride
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid AdminUserId { get; set; }
    public AdminActionType ActionType { get; set; }
    public string RefType { get; set; } = string.Empty;
    public Guid RefId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
