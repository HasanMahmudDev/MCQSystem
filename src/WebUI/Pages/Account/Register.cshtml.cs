using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Account;

public sealed class RegisterModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await authService.RegisterStudentAsync(new RegisterRequest(Input.FullName, Input.Email, Input.Password), cancellationToken);
        if (!result.Succeeded || result.Value is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed.");
            return Page();
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, CreatePrincipal(result.Value));
        return RedirectToPage("/Student/Exams/Index");
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

    public sealed class RegisterInput
    {
        [Required]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
