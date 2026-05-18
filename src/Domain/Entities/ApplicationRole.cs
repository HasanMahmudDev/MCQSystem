using MCQSystem.Domain.Common;

namespace MCQSystem.Domain.Entities;

public sealed class ApplicationRole : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;
}
