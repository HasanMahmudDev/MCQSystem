using MCQSystem.Domain.Common;

namespace MCQSystem.Domain.Entities;

public sealed class ApplicationUser : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string NormalizedEmail { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = [];

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAtUtc { get; set; }
}
