namespace MCQSystem.Domain.Common;

public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
}
