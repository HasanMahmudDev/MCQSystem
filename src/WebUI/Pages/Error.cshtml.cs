using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages;

public sealed class ErrorModel : PageModel
{
    public string Message { get; private set; } = "An unexpected error occurred.";

    public void OnGet(string? message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            Message = message;
        }
    }
}
