using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Departments.UpdateLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateLocationsTests : BaseIntegrationTest
{
    public UpdateLocationsTests(DirectoryServiceTestsWebFactory webFactory)
        : base(webFactory)
    {
    }

    [Fact]
    public async Task UpdateLocations_WithValidLocation_ShouldBeSucceed()
    {
        // arrange
        var initialLocationId = await CreateLocation();
        var newLocationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        var departmentId = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateRoot([initialLocationId.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        // act
        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId.Value,
                [newLocationId.Value]);

            return sut.HandleAsync(command, cancellationToken);
        });

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.DepartmentLocations)
                .ThenInclude(dl => dl.Location)
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId.Value), cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(department.Id.Value, result.Value);
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            Assert.Single(department.DepartmentLocations);
            Assert.Equal(newLocationId.Value, department.DepartmentLocations.First().Location.Id.Value);
        });
    }

    [Fact]
    public async Task UpdateLocations_WithValidLocations_ShouldBeSucceed()
    {
        // arrange
        var initialLocationId = await CreateLocation();
        var newLocationId1 = await CreateLocation();
        var newLocationId2 = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        var departmentId = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateRoot([initialLocationId.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        // act
        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId.Value,
                [newLocationId1.Value, newLocationId2.Value]);

            return sut.HandleAsync(command, cancellationToken);
        });

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.DepartmentLocations)
                .ThenInclude(dl => dl.Location)
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId.Value), cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            Assert.Equal(2, department.DepartmentLocations.Count);
            Assert.Contains(department.DepartmentLocations, dl => dl.Location.Id.Value == newLocationId1.Value);
            Assert.Contains(department.DepartmentLocations, dl => dl.Location.Id.Value == newLocationId2.Value);
        });
    }

    [Fact]
    public async Task UpdateLocations_WithNoLocations_ShouldBeFailed()
    {
        // arrange
        var initialLocationId = await CreateLocation();

        var cancellationToken = CancellationToken.None;

        var departmentId = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateRoot([initialLocationId.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        // act
        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId.Value,
                []);

            return sut.HandleAsync(command, cancellationToken);
        });

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.DepartmentLocations)
                .ThenInclude(dl => dl.Location)
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId.Value), cancellationToken);

            Assert.True(result.IsFailure);
            Assert.NotEmpty(department.DepartmentLocations);
        });
    }
}