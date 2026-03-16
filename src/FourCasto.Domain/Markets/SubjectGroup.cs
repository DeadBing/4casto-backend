namespace FourCasto.Domain.Markets;

public class SubjectGroup
{
    public Guid Id { get; set; }
    public Guid FourCastoWlId { get; set; }
    public string Name { get; set; } = string.Empty; // FX, Crypto, Politics, Macro, Sports
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
