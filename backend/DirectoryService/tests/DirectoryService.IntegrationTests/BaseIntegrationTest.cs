using DirectoryService.Domain.Locations;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.IntegrationTests.Fakers;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<DirectoryServiceTestsWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabaseAsync;
    private readonly IServiceProvider _services;

    protected BaseIntegrationTest(DirectoryServiceTestsWebFactory webFactory)
    {
        _services = webFactory.Services;
        _resetDatabaseAsync = webFactory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabaseAsync();

    protected async Task<LocationId> CreateLocation()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = LocationFaker
                .Default
                .Generate();

            dbContext.Locations.Add(location);
            await dbContext.SaveChangesAsync();

            return location.Id;
        });
    }

    protected async Task<TResult> ExecuteHandler<THandler, TResult>(Func<THandler, Task<TResult>> func)
        where THandler : notnull
    {
        await using var scope = _services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<THandler>();
        return await func(sut);
    }

    protected async Task<TResult> ExecuteInDb<TResult>(Func<DirectoryServiceDbContext, Task<TResult>> func)
    {
        await using var scope = _services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        return await func(dbContext);
    }

    protected async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> func)
    {
        await using var scope = _services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        await func(dbContext);
    }
}