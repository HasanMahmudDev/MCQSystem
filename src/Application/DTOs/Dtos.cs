using MCQSystem.Domain.Enums;

namespace MCQSystem.Application.DTOs;

public sealed record CategoryDto(
    string Id,
    string Name,
    string Slug,
    string? Description,
    bool IsActive);

public sealed record ExamDto(
    string Id,
    string Title,
    string? Description,
    string CategoryId,
    int DurationMinutes,
    decimal TotalMarks,
    decimal PassMarks,
    int NumberOfQuestions,
    bool IsPublished,
    bool RandomizeQuestions,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc);

public sealed record QuestionOptionDto(string Key, string Text);

public sealed record QuestionDto(
    string Id,
    string ExamId,
    string CategoryId,
    string Text,
    List<QuestionOptionDto> Options,
    string CorrectOptionKey,
    DifficultyLevel Difficulty,
    decimal Marks,
    decimal NegativeMarks,
    string? Explanation,
    bool IsActive);

public sealed record UserDto(
    string Id,
    string FullName,
    string Email,
    IReadOnlyList<string> Roles,
    bool IsActive,
    DateTime? LastLoginAtUtc);

public sealed record ResultDto(
    string Id,
    string UserId,
    string UserName,
    string ExamId,
    string ExamTitle,
    int TotalQuestions,
    int Attempted,
    int CorrectAnswers,
    int WrongAnswers,
    decimal TotalMarks,
    decimal ObtainedMarks,
    decimal Percentage,
    bool Passed,
    DateTime SubmittedAtUtc);

public sealed record ChartPointDto(string Label, int Value, string BarClass);

public sealed record ActivityDto(string Title, string Description, string Tone);

public sealed record DashboardDto(
    long TotalUsers,
    long PublishedExams,
    long QuestionCount,
    long ResultsCount,
    decimal AverageScore,
    IReadOnlyList<ResultDto> RecentResults,
    IReadOnlyList<ResultDto> Leaderboard,
    IReadOnlyList<ChartPointDto> SubmissionTrend,
    IReadOnlyList<ActivityDto> RecentActivities);

public sealed record LoginRequest(string Email, string Password, bool RememberMe);

public sealed record RegisterRequest(string FullName, string Email, string Password);

public sealed record AuthenticatedUserDto(
    string Id,
    string FullName,
    string Email,
    IReadOnlyList<string> Roles);

public sealed record StudentExamSummaryDto(
    string Id,
    string Title,
    string? Description,
    string CategoryName,
    int DurationMinutes,
    int QuestionCount,
    decimal TotalMarks,
    decimal PassMarks,
    bool IsOpen);

public sealed record ExamAttemptDto(
    string ExamId,
    string Title,
    string? Description,
    int DurationMinutes,
    DateTime StartedAtUtc,
    IReadOnlyList<QuestionAttemptDto> Questions);

public sealed record QuestionAttemptDto(
    string Id,
    string Text,
    IReadOnlyList<QuestionOptionDto> Options,
    decimal Marks);

public sealed record AnswerSubmissionDto(string QuestionId, string? SelectedOptionKey);

public sealed record SubmitExamRequest(
    string ExamId,
    DateTime StartedAtUtc,
    IReadOnlyList<string> QuestionIds,
    IReadOnlyList<AnswerSubmissionDto> Answers);
