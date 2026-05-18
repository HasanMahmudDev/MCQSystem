using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Student.Exams;

public sealed class IndexModel(IExamService examService) : PageModel
{
    public IReadOnlyList<StudentExamSummaryDto> Exams { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Exams = await examService.GetPublishedExamsAsync(cancellationToken);
    }
}
