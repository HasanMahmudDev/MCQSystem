using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCQSystem.WebUI.Pages.Student.Leaderboard;

public sealed class IndexModel(
    IRepository<Exam> exams,
    IExamService examService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? ExamId { get; set; }

    public IReadOnlyList<ResultDto> Leaderboard { get; private set; } = [];

    public List<SelectListItem> ExamOptions { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var published = (await exams.ListAsync(x => x.IsPublished, cancellationToken))
            .OrderBy(x => x.Title)
            .ToList();

        ExamOptions = published
            .Select(x => new SelectListItem(x.Title, x.Id, x.Id == ExamId))
            .ToList();

        if (string.IsNullOrWhiteSpace(ExamId) && published.Count > 0)
        {
            ExamId = published[0].Id;
        }

        if (!string.IsNullOrWhiteSpace(ExamId))
        {
            Leaderboard = await examService.GetLeaderboardAsync(ExamId, cancellationToken);
        }
    }
}
