using System.ComponentModel.DataAnnotations;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using MCQSystem.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Admin.Users;

public sealed class IndexModel(
    IRepository<ApplicationUser> users,
    IPasswordHashService passwordHashService) : PageModel
{
    public IReadOnlyList<ApplicationUser> Items { get; private set; } = [];

    [BindProperty]
    public CreateUserInput Input { get; set; } = new();

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

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var normalizedEmail = Input.Email.Trim().ToUpperInvariant();
        if (await users.CountAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken) > 0)
        {
            ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.Email)}", "An account already exists with this email.");
            await LoadAsync(cancellationToken);
            return Page();
        }

        var user = new ApplicationUser
        {
            FullName = Input.FullName.Trim(),
            Email = Input.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            Roles = [Input.Role],
            IsActive = true
        };
        user.PasswordHash = passwordHashService.HashPassword(user, Input.Password);
        await users.AddAsync(user, cancellationToken);

        TempData["Toast"] = "User created successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(string id, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(id, cancellationToken);
        if (user is not null)
        {
            user.IsActive = !user.IsActive;
            await users.UpdateAsync(user, cancellationToken);
            TempData["Toast"] = "User status updated.";
        }

        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        var data = await users.ListAsync(cancellationToken: cancellationToken);
        if (!string.IsNullOrWhiteSpace(Search))
        {
            data = data
                .Where(x => x.FullName.Contains(Search, StringComparison.OrdinalIgnoreCase)
                    || x.Email.Contains(Search, StringComparison.OrdinalIgnoreCase)
                    || x.Roles.Any(role => role.Contains(Search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        var ordered = data.OrderBy(x => x.FullName).ToList();
        TotalPages = Math.Max(1, (int)Math.Ceiling(ordered.Count / (double)PageSize));
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Items = ordered.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
    }

    public sealed class CreateUserInput
    {
        [Required]
        [MaxLength(120)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = AppRoles.Student;
    }
}
