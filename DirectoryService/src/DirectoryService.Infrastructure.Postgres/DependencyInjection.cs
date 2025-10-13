using DirectoryService.Application.Locations;
using DirectoryService.Infrastructure.Postgres.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(_ => new DirectoryServiceDbContext(configuration["DirectoryServiceDb"]!));
        services.AddScoped<ILocationsRepository, EfCoreLocationsRepository>();

        return services;
    }
}
