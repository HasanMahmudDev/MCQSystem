using MCQSystem.Domain.Common;

namespace MCQSystem.Domain.Entities;

public sealed class Exam : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string CategoryId { get; set; } = string.Empty;

    public int DurationMinutes { get; set; } = 30;

    public decimal TotalMarks { get; set; }

    public decimal PassMarks { get; set; }

    public int NumberOfQuestions { get; set; } = 10;

    public bool IsPublished { get; set; }

    public bool RandomizeQuestions { get; set; } = true;

    public DateTime? StartsAtUtc { get; set; }

    public DateTime? EndsAtUtc { get; set; }

    public List<string> QuestionIds { get; set; } = [];
}
