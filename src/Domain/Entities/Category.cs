using MCQSystem.Domain.Common;

namespace MCQSystem.Domain.Entities;

public sealed class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
