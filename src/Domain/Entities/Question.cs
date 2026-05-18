using MCQSystem.Domain.Common;
using MCQSystem.Domain.Enums;

namespace MCQSystem.Domain.Entities;

public sealed class Question : BaseEntity
{
    public string ExamId { get; set; } = string.Empty;

    public string CategoryId { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public List<QuestionOption> Options { get; set; } = [];

    public string CorrectOptionKey { get; set; } = "A";

    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

    public decimal Marks { get; set; } = 1;

    public decimal NegativeMarks { get; set; }

    public string? Explanation { get; set; }

    public bool IsActive { get; set; } = true;
}

public sealed class QuestionOption
{
    public string Key { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}
