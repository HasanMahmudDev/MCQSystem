using MCQSystem.Application.Interfaces;
using MCQSystem.Persistence.Repositories;
using MCQSystem.Persistence.Seed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MCQSystem.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(settings =>
        {
            settings.ConnectionString = configuration["MongoDb:ConnectionString"] ?? settings.ConnectionString;
            settings.DatabaseName = configuration["MongoDb:DatabaseName"] ?? settings.DatabaseName;
        });
        services.AddSingleton<MongoDbContext>();
        services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
        services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        return services;
    }
}
