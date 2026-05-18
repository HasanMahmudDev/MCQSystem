using System.Security.Claims;
using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Student.Exams;

public sealed class StartModel(IExamService examService) : PageModel
{
    public ExamAttemptDto? Attempt { get; private set; }

    [BindProperty]
    public string ExamId { get; set; } = string.Empty;

    [BindProperty]
    public DateTime StartedAtUtc { get; set; }

    [BindProperty]
    public List<string> QuestionIds { get; set; } = [];

    [BindProperty]
    public Dictionary<string, string?> Answers { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string examId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var result = await examService.StartExamAsync(examId, userId, cancellationToken);
        if (!result.Succeeded || result.Value is null)
        {
            TempData["Toast"] = result.Error ?? "Exam is not available.";
            return RedirectToPage("/Student/Exams/Index");
        }

        Attempt = result.Value;
        ExamId = result.Value.ExamId;
        StartedAtUtc = result.Value.StartedAtUtc;
        QuestionIds = result.Value.Questions.Select(x => x.Id).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var submissions = QuestionIds
            .Select(questionId => new AnswerSubmissionDto(
                questionId,
                Answers.TryGetValue(questionId, out var selected) ? selected : null))
            .ToList();

        var request = new SubmitExamRequest(ExamId, StartedAtUtc, QuestionIds, submissions);
        var result = await examService.SubmitExamAsync(userId, request, cancellationToken);
        if (!result.Succeeded || result.Value is null)
        {
            TempData["Toast"] = result.Error ?? "Exam submission failed.";
            return RedirectToPage("/Student/Exams/Index");
        }

        return RedirectToPage("/Student/Exams/Result", new { resultId = result.Value.Id });
    }
}
