using MCQSystem.Domain.Common;

namespace MCQSystem.Domain.Entities;

public sealed class Result : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string ExamId { get; set; } = string.Empty;

    public string ExamTitle { get; set; } = string.Empty;

    public int TotalQuestions { get; set; }

    public int Attempted { get; set; }

    public int CorrectAnswers { get; set; }

    public int WrongAnswers { get; set; }

    public decimal TotalMarks { get; set; }

    public decimal ObtainedMarks { get; set; }

    public decimal Percentage { get; set; }

    public bool Passed { get; set; }

    public DateTime StartedAtUtc { get; set; }

    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;

    public int DurationSeconds { get; set; }
}
