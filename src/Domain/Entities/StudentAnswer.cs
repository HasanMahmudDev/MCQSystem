using MCQSystem.Domain.Common;

namespace MCQSystem.Domain.Entities;

public sealed class StudentAnswer : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public string ExamId { get; set; } = string.Empty;

    public string QuestionId { get; set; } = string.Empty;

    public string? SelectedOptionKey { get; set; }

    public bool IsCorrect { get; set; }

    public decimal MarksAwarded { get; set; }

    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
}
