using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MCQSystem.Infrastructure.Authentication;

public sealed class PasswordHashService : IPasswordHashService
{
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    public string HashPassword(ApplicationUser user, string password)
        => _passwordHasher.HashPassword(user, password);

    public bool VerifyPassword(ApplicationUser user, string password)
        => _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
}
