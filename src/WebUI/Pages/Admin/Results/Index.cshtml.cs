using System.Text;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Admin.Results;

public sealed class IndexModel(IRepository<Result> results) : PageModel
{
    public IReadOnlyList<Result> Items { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; private set; }

    public int PageSize { get; } = 10;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnGetExportAsync(CancellationToken cancellationToken)
    {
        var data = await results.ListAsync(cancellationToken: cancellationToken);
        var builder = new StringBuilder();
        builder.AppendLine("Student,Exam,Attempted,Correct,Wrong,TotalMarks,ObtainedMarks,Percentage,Passed,SubmittedAtUtc");

        foreach (var result in data.OrderByDescending(x => x.SubmittedAtUtc))
        {
            builder.AppendLine(string.Join(',',
                Escape(result.UserName),
                Escape(result.ExamTitle),
                result.Attempted,
                result.CorrectAnswers,
                result.WrongAnswers,
                result.TotalMarks,
                result.ObtainedMarks,
                result.Percentage,
                result.Passed,
                result.SubmittedAtUtc.ToString("O")));
        }

        return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "mcq-results.csv");
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        var data = await results.ListAsync(cancellationToken: cancellationToken);
        if (!string.IsNullOrWhiteSpace(Search))
        {
            data = data
                .Where(x => x.UserName.Contains(Search, StringComparison.OrdinalIgnoreCase)
                    || x.ExamTitle.Contains(Search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var ordered = data.OrderByDescending(x => x.SubmittedAtUtc).ToList();
        TotalPages = Math.Max(1, (int)Math.Ceiling(ordered.Count / (double)PageSize));
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Items = ordered.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
    }

    private static string Escape(string value)
        => $"\"{value.Replace("\"", "\"\"")}\"";
}
