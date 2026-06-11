using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Postgres.Seeding;

public static class SeederExtensions
{
    extension(IServiceProvider serviceProvider)
    {
        public async Task<IServiceProvider> RunSeeding()
        {
            using var scope = serviceProvider.CreateScope();

            var seeders = scope.ServiceProvider.GetServices<DirectoryServiceSeeder>();

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync();
            }

            return serviceProvider;
        }
    }
}