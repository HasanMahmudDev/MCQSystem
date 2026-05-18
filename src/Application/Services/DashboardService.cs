using AutoMapper;
using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;

namespace MCQSystem.Application.Services;

public sealed class DashboardService(
    IRepository<ApplicationUser> users,
    IRepository<Exam> exams,
    IRepository<Question> questions,
    IRepository<Result> results,
    IMapper mapper) : IDashboardService
{
    public async Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var allResults = await results.ListAsync(cancellationToken: cancellationToken);
        var recent = allResults
            .OrderByDescending(x => x.SubmittedAtUtc)
            .Take(6)
            .Select(mapper.Map<ResultDto>)
            .ToList();
        var leaderboard = allResults
            .OrderByDescending(x => x.ObtainedMarks)
            .ThenBy(x => x.DurationSeconds)
            .Take(6)
            .Select(mapper.Map<ResultDto>)
            .ToList();
        var averageScore = allResults.Count == 0 ? 0 : Math.Round(allResults.Average(x => x.Percentage), 2);
        var submissionTrend = BuildSubmissionTrend(allResults);
        var activities = BuildRecentActivities(allResults);

        return new DashboardDto(
            await users.CountAsync(cancellationToken: cancellationToken),
            await exams.CountAsync(x => x.IsPublished, cancellationToken),
            await questions.CountAsync(x => x.IsActive, cancellationToken),
            allResults.Count,
            averageScore,
            recent,
            leaderboard,
            submissionTrend,
            activities);
    }

    private static IReadOnlyList<ChartPointDto> BuildSubmissionTrend(IReadOnlyList<Result> results)
    {
        var barClasses = new[] { "bar-42", "bar-51", "bar-58", "bar-66", "bar-72", "bar-83" };
        var months = Enumerable.Range(0, 6)
            .Select(offset => DateTime.UtcNow.AddMonths(-5 + offset))
            .Select(date => new DateTime(date.Year, date.Month, 1))
            .ToList();

        var counts = months
            .Select(month => results.Count(x =>
                x.SubmittedAtUtc.Year == month.Year &&
                x.SubmittedAtUtc.Month == month.Month))
            .ToList();

        return months
            .Select((month, index) => new ChartPointDto(
                month.ToString("MMM"),
                counts[index],
                barClasses[Math.Min(index, barClasses.Length - 1)]))
            .ToList();
    }

    private static IReadOnlyList<ActivityDto> BuildRecentActivities(IReadOnlyList<Result> results)
    {
        return results
            .OrderByDescending(x => x.SubmittedAtUtc)
            .Take(4)
            .Select(result => new ActivityDto(
                $"{result.UserName} submitted {result.ExamTitle}",
                $"{result.ObtainedMarks}/{result.TotalMarks} marks ({result.Percentage}%)",
                result.Passed ? "success" : "warning"))
            .ToList();
    }
}
