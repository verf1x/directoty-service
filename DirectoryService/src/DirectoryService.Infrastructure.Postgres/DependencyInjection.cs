using DirectoryService.Application.Departments;
using DirectoryService.Application.Locations;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Infrastructure.Postgres.Departments;
using DirectoryService.Infrastructure.Postgres.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DirectoryServiceDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration["DirectoryServiceDb"]);

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            options.UseLoggerFactory(loggerFactory);
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();

        services.AddScoped<ILocationsCommandRepository, EfCoreLocationsCommandRepository>();
        services.AddScoped<ILocationsQueryRepository, SqlLocationsQueryRepository>();
        services.AddScoped<IDepartmentsCommandRepository, EfCoreDepartmentsCommandRepository>();
        services.AddScoped<IDepartmentsQueryRepository, SqlDepartmentsQueryRepository>();

        return services;
    }
}