using FluentValidation;
using MCQSystem.Application.Interfaces;
using MCQSystem.Application.Mappings;
using MCQSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MCQSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(configuration => configuration.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
