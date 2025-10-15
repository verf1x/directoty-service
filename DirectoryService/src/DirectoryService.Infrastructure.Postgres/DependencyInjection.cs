using DirectoryService.Application.Locations;
using DirectoryService.Infrastructure.Postgres.Repositories;
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

        services.AddScoped<ILocationsRepository, EfCoreLocationsRepository>();

        return services;
    }
}