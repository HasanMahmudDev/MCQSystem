using System.Security.Claims;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Student.Profile;

public sealed class IndexModel(IRepository<ApplicationUser> users) : PageModel
{
    public ApplicationUser? CurrentUser { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        CurrentUser = await users.GetByIdAsync(userId, cancellationToken);
    }
}
