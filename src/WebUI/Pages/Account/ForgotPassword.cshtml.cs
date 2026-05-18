using System.ComponentModel.DataAnnotations;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Account;

public sealed class ForgotPasswordModel(
    IRepository<ApplicationUser> users,
    IEmailService emailService) : PageModel
{
    [BindProperty]
    public ForgotPasswordInput Input { get; set; } = new();

    public string? StatusMessage { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var normalizedEmail = Input.Email.Trim().ToUpperInvariant();
        var user = (await users.ListAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken)).SingleOrDefault();
        if (user is not null)
        {
            await emailService.SendAsync(
                user.Email,
                "MCQ System password reset",
                $"Hello {user.FullName}, use your administrator to reset your password for {user.Email}.",
                cancellationToken);
        }

        StatusMessage = "If an account exists for that email, a reset message has been sent.";
        return Page();
    }

    public sealed class ForgotPasswordInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
