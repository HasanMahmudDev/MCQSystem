using System.Security.Claims;
using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Student.History;

public sealed class IndexModel(IExamService examService) : PageModel
{
    public IReadOnlyList<ResultDto> Results { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        Results = await examService.GetHistoryAsync(userId, cancellationToken);
    }
}
