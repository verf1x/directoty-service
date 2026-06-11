using DirectoryService.Application.Database;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Positions;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Infrastructure.Postgres.Departments;
using DirectoryService.Infrastructure.Postgres.Locations;
using DirectoryService.Infrastructure.Postgres.Positions;
using DirectoryService.Infrastructure.Postgres.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddScoped<ITransactionManager, TransactionManager>();

            string connectionString = configuration.GetConnectionString("DirectoryServiceDb")
                                      ?? throw new InvalidOperationException(
                                          "Connection string 'DirectoryServiceDb' is not configured.");

            services.AddDbContext<DirectoryServiceDbContext>((serviceProvider, options) =>
            {
                options.UseNpgsql(connectionString);

                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                options.UseLoggerFactory(loggerFactory);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();

            services.AddScoped<ILocationsRepository, LocationsRepository>();
            services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
            services.AddScoped<IPositionsRepository, PositionsRepository>();
            services.AddScoped<DirectoryServiceSeeder>();

            return services;
        }
    }
}
