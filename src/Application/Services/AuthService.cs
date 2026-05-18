using MCQSystem.Application.Common;
using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using MCQSystem.Shared;

namespace MCQSystem.Application.Services;

public sealed class AuthService(
    IRepository<ApplicationUser> users,
    IPasswordHashService passwordHashService) : IAuthService
{
    public async Task<ServiceResult<AuthenticatedUserDto>> SignInAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var user = (await users.ListAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken)).SingleOrDefault();

        if (user is null || !user.IsActive || !passwordHashService.VerifyPassword(user, request.Password))
        {
            return ServiceResult<AuthenticatedUserDto>.Failure("Invalid email or password.");
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        user.UpdatedAtUtc = DateTime.UtcNow;
        await users.UpdateAsync(user, cancellationToken);

        return ServiceResult<AuthenticatedUserDto>.Success(ToAuthenticatedUser(user));
    }

    public async Task<ServiceResult<AuthenticatedUserDto>> RegisterStudentAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var exists = await users.CountAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken) > 0;

        if (exists)
        {
            return ServiceResult<AuthenticatedUserDto>.Failure("An account already exists with this email address.");
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            Roles = [AppRoles.Student],
            IsActive = true
        };
        user.PasswordHash = passwordHashService.HashPassword(user, request.Password);

        await users.AddAsync(user, cancellationToken);

        return ServiceResult<AuthenticatedUserDto>.Success(ToAuthenticatedUser(user));
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();

    private static AuthenticatedUserDto ToAuthenticatedUser(ApplicationUser user)
        => new(user.Id, user.FullName, user.Email, user.Roles);
}
