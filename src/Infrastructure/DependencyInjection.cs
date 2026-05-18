using MCQSystem.Application.Interfaces;
using MCQSystem.Infrastructure.Authentication;
using MCQSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MCQSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHashService, PasswordHashService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}
