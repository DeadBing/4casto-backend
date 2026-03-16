namespace FourCasto.Domain.Settlement;

using FourCasto.Contracts.Enums;

public class ResolutionSource
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ResolutionSourceType SourceType { get; set; }
    public string? Config { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
