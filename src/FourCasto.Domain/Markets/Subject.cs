namespace FourCasto.Domain.Markets;

public class Subject
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public Guid SubjectGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public SubjectGroup SubjectGroup { get; set; } = null!;
}
