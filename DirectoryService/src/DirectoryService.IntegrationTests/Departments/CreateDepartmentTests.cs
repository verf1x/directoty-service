using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests : BaseIntegrationTest
{
    public CreateDepartmentTests(DirectoryServiceTestsWebFactory webFactory)
        : base(webFactory)
    {
    }

    [Fact]
    public async Task CreateDepartment_WithValidData_ShouldBeSucceed()
    {
        // arrange
        var locationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
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
        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
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
        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
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
        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
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
}