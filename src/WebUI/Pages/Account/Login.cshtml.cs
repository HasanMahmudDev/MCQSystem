using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using MCQSystem.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Account;

public sealed class LoginModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Page();
        }

        return RedirectToPage(User.IsInRole(AppRoles.Admin) ? "/Admin/Dashboard/Index" : "/Student/Exams/Index");
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await authService.SignInAsync(new LoginRequest(Input.Email, Input.Password, Input.RememberMe), cancellationToken);
        if (!result.Succeeded || result.Value is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Login failed.");
            return Page();
        }

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            CreatePrincipal(result.Value),
            new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe,
                ExpiresUtc = Input.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
            });

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl);
        }

        return RedirectToPage(result.Value.Roles.Contains(AppRoles.Admin) ? "/Admin/Dashboard/Index" : "/Student/Exams/Index");
    }

    private static ClaimsPrincipal CreatePrincipal(AuthenticatedUserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }

    public sealed class LoginInput
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
