using AutoMapper;
using MCQSystem.Application.Common;
using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;

namespace MCQSystem.Application.Services;

public sealed class ExamService(
    IRepository<Exam> exams,
    IRepository<Question> questions,
    IRepository<Category> categories,
    IRepository<Result> results,
    IRepository<StudentAnswer> answers,
    IRepository<ApplicationUser> users,
    IMapper mapper) : IExamService
{
    public async Task<IReadOnlyList<StudentExamSummaryDto>> GetPublishedExamsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var published = await exams.ListAsync(x =>
            x.IsPublished &&
            (!x.StartsAtUtc.HasValue || x.StartsAtUtc <= now) &&
            (!x.EndsAtUtc.HasValue || x.EndsAtUtc >= now), cancellationToken);
        var allCategories = await categories.ListAsync(cancellationToken: cancellationToken);
        var allQuestions = await questions.ListAsync(q => q.IsActive, cancellationToken);

        return published
            .OrderBy(x => x.Title)
            .Select(exam =>
            {
                var categoryName = allCategories.FirstOrDefault(x => x.Id == exam.CategoryId)?.Name ?? "General";
                var count = CountExamQuestions(exam, allQuestions);
                var totalMarks = allQuestions.Where(q => BelongsToExam(exam, q)).Sum(q => q.Marks);

                return new StudentExamSummaryDto(
                    exam.Id,
                    exam.Title,
                    exam.Description,
                    categoryName,
                    exam.DurationMinutes,
                    count,
                    totalMarks,
                    exam.PassMarks,
                    true);
            })
            .ToList();
    }

    public async Task<ServiceResult<ExamAttemptDto>> StartExamAsync(string examId, string userId, CancellationToken cancellationToken = default)
    {
        var exam = await exams.GetByIdAsync(examId, cancellationToken);
        if (exam is null || !exam.IsPublished)
        {
            return ServiceResult<ExamAttemptDto>.Failure("Exam is not available.");
        }

        var sourceQuestions = await GetExamQuestionsAsync(exam, cancellationToken);
        if (sourceQuestions.Count == 0)
        {
            return ServiceResult<ExamAttemptDto>.Failure("This exam has no active questions.");
        }

        var selected = exam.RandomizeQuestions
            ? sourceQuestions.OrderBy(_ => Guid.NewGuid()).ToList()
            : sourceQuestions.ToList();

        selected = selected.Take(Math.Min(exam.NumberOfQuestions, selected.Count)).ToList();

        var attempt = new ExamAttemptDto(
            exam.Id,
            exam.Title,
            exam.Description,
            exam.DurationMinutes,
            DateTime.UtcNow,
            selected.Select(q => new QuestionAttemptDto(
                q.Id,
                q.Text,
                q.Options.OrderBy(o => o.Key).Select(o => mapper.Map<QuestionOptionDto>(o)).ToList(),
                q.Marks)).ToList());

        return ServiceResult<ExamAttemptDto>.Success(attempt);
    }

    public async Task<ServiceResult<ResultDto>> SubmitExamAsync(string userId, SubmitExamRequest request, CancellationToken cancellationToken = default)
    {
        var exam = await exams.GetByIdAsync(request.ExamId, cancellationToken);
        if (exam is null)
        {
            return ServiceResult<ResultDto>.Failure("Exam was not found.");
        }

        var user = await users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<ResultDto>.Failure("Student account was not found.");
        }

        var questionIdSet = request.QuestionIds.Count == 0
            ? null
            : request.QuestionIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var examQuestions = (await GetExamQuestionsAsync(exam, cancellationToken))
            .Where(q => questionIdSet is null || questionIdSet.Contains(q.Id))
            .ToList();
        var submittedAnswers = request.Answers
            .GroupBy(a => a.QuestionId)
            .ToDictionary(g => g.Key, g => g.Last().SelectedOptionKey, StringComparer.OrdinalIgnoreCase);

        var attempted = 0;
        var correct = 0;
        var wrong = 0;
        decimal obtained = 0;
        var totalMarks = examQuestions.Sum(q => q.Marks);

        foreach (var question in examQuestions)
        {
            submittedAnswers.TryGetValue(question.Id, out var selectedOption);
            var hasAnswer = !string.IsNullOrWhiteSpace(selectedOption);
            var isCorrect = hasAnswer && string.Equals(selectedOption, question.CorrectOptionKey, StringComparison.OrdinalIgnoreCase);
            var marksAwarded = isCorrect ? question.Marks : hasAnswer ? -question.NegativeMarks : 0;

            if (hasAnswer)
            {
                attempted++;
                if (isCorrect)
                {
                    correct++;
                }
                else
                {
                    wrong++;
                }
            }

            obtained += marksAwarded;

            await answers.AddAsync(new StudentAnswer
            {
                UserId = userId,
                ExamId = exam.Id,
                QuestionId = question.Id,
                SelectedOptionKey = selectedOption,
                IsCorrect = isCorrect,
                MarksAwarded = marksAwarded
            }, cancellationToken);
        }

        var submittedAt = DateTime.UtcNow;
        var percentage = totalMarks <= 0 ? 0 : Math.Round(obtained / totalMarks * 100, 2);
        var passMark = exam.PassMarks > 0 ? exam.PassMarks : totalMarks * 0.5m;

        var result = new Result
        {
            UserId = userId,
            UserName = user.FullName,
            ExamId = exam.Id,
            ExamTitle = exam.Title,
            TotalQuestions = examQuestions.Count,
            Attempted = attempted,
            CorrectAnswers = correct,
            WrongAnswers = wrong,
            TotalMarks = totalMarks,
            ObtainedMarks = obtained,
            Percentage = percentage,
            Passed = obtained >= passMark,
            StartedAtUtc = request.StartedAtUtc,
            SubmittedAtUtc = submittedAt,
            DurationSeconds = Math.Max(0, (int)(submittedAt - request.StartedAtUtc).TotalSeconds)
        };

        await results.AddAsync(result, cancellationToken);

        return ServiceResult<ResultDto>.Success(mapper.Map<ResultDto>(result));
    }

    public async Task<IReadOnlyList<ResultDto>> GetHistoryAsync(string userId, CancellationToken cancellationToken = default)
    {
        var history = await results.ListAsync(x => x.UserId == userId, cancellationToken);
        return history.OrderByDescending(x => x.SubmittedAtUtc).Select(mapper.Map<ResultDto>).ToList();
    }

    public async Task<IReadOnlyList<ResultDto>> GetLeaderboardAsync(string examId, CancellationToken cancellationToken = default)
    {
        var leaderboard = await results.ListAsync(x => x.ExamId == examId, cancellationToken);
        return leaderboard
            .OrderByDescending(x => x.ObtainedMarks)
            .ThenBy(x => x.DurationSeconds)
            .Take(20)
            .Select(mapper.Map<ResultDto>)
            .ToList();
    }

    private async Task<IReadOnlyList<Question>> GetExamQuestionsAsync(Exam exam, CancellationToken cancellationToken)
    {
        var allQuestions = await questions.ListAsync(q => q.IsActive, cancellationToken);
        return allQuestions.Where(q => BelongsToExam(exam, q)).ToList();
    }

    private static bool BelongsToExam(Exam exam, Question question)
        => question.ExamId == exam.Id || exam.QuestionIds.Contains(question.Id, StringComparer.OrdinalIgnoreCase);

    private static int CountExamQuestions(Exam exam, IReadOnlyList<Question> allQuestions)
        => allQuestions.Count(q => BelongsToExam(exam, q));
}
