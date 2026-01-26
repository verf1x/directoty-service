using DirectoryService.Application.Departments.Create;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.IntegrationTests.Fakers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests;

public class CreateDepartmentTests : IClassFixture<DirectoryServiceTestsWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabaseAsync;

    public CreateDepartmentTests(DirectoryServiceTestsWebFactory webFactory)
    {
        Services = webFactory.Services;
        _resetDatabaseAsync = webFactory.ResetDatabaseAsync;
    }

    private IServiceProvider Services { get; set; }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabaseAsync();

    [Fact]
    public async Task CreateDepartment_WithValidData_ShouldBeSucceed()
    {
        // arrange
        var locationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateRoot([locationId.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(result.Value), cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(department.Id.Value, result.Value);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task CreateDepartment_WithTwoLocationsAndValidData_ShouldBeSucceed()
    {
        // arrange
        var locationId1 = await CreateLocation();
        var locationId2 = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateRoot([locationId1.Value, locationId2.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(result.Value), cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(department.Id.Value, result.Value);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task CreateDepartment_WithEmptyLocationsIds_ShouldBeFailedWithValidationError()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateRoot([])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstOrDefaultAsync(cancellationToken);

            Assert.Null(department);

            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Error);
        });
    }

    [Fact]
    public async Task CreateDepartment_WithNonExistentLocationId_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler(sut =>
        {
            var nonExistentLocationId = Guid.CreateVersion7();

            var command = CreateDepartmentCommandFaker
                .CreateRoot([nonExistentLocationId])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstOrDefaultAsync(cancellationToken);

            Assert.Null(department);

            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Error);
        });
    }

    private async Task<LocationId> CreateLocation()
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

    private async Task<T> ExecuteHandler<T>(Func<CreateDepartmentHandler, Task<T>> func)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<CreateDepartmentHandler>();
        return await func(sut);
    }

    private async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> func)
    {
        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        return await func(dbContext);
    }

    private async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> func)
    {
        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        await func(dbContext);
    }
}