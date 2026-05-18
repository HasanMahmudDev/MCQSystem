using MCQSystem.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages;

public sealed class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Account/Login");
        }

        return RedirectToPage(User.IsInRole(AppRoles.Admin) ? "/Admin/Dashboard/Index" : "/Student/Exams/Index");
    }
}
