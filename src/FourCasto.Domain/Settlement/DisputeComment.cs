namespace FourCasto.Domain.Settlement;

public class DisputeComment
{
    public Guid Id { get; set; }
    public Guid DisputeCaseId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorRole { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DisputeCase DisputeCase { get; set; } = null!;
}
