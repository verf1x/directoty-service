using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Departments.SoftDelete;
using DirectoryService.Application.Positions.Create;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class SoftDeleteDepartmentTests : BaseIntegrationTest
{
    public SoftDeleteDepartmentTests(DirectoryServiceTestsWebFactory webFactory)
        : base(webFactory)
    {
    }

    [Fact]
    public async Task SoftDelete_WithExclusiveLocationsAndPositions_DeactivatesDepartmentAndExclusiveRelationsOnly()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var exclusiveLocationId = await CreateLocation();
        var sharedLocationId = await CreateLocation();

        var departmentId = await CreateDepartment([exclusiveLocationId.Value, sharedLocationId.Value], cancellationToken);
        var otherDepartmentId = await CreateDepartment([sharedLocationId.Value], cancellationToken);

        var exclusivePositionId = await CreatePosition([departmentId], cancellationToken);
        var sharedPositionId = await CreatePosition([departmentId, otherDepartmentId], cancellationToken);

        var deletedFrom = DateTime.UtcNow;

        // act
        var result = await ExecuteHandler<SoftDeleteDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new SoftDeleteDepartmentCommand(departmentId);

            return sut.HandleAsync(command, cancellationToken);
        });

        var deletedTo = DateTime.UtcNow;

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .Include(d => d.DepartmentLocations)
                .Include(d => d.DepartmentPositions)
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId), cancellationToken);

            var exclusiveLocation = await dbContext.Locations
                .FirstAsync(l => l.Id == LocationId.Create(exclusiveLocationId.Value), cancellationToken);

            var sharedLocation = await dbContext.Locations
                .FirstAsync(l => l.Id == LocationId.Create(sharedLocationId.Value), cancellationToken);

            var exclusivePosition = await dbContext.Positions
                .FirstAsync(p => p.Id == PositionId.Create(exclusivePositionId), cancellationToken);

            var sharedPosition = await dbContext.Positions
                .FirstAsync(p => p.Id == PositionId.Create(sharedPositionId), cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.Equal(departmentId, result.Value);
            Assert.False(department.IsActive);
            Assert.NotNull(department.DeletedAt);
            Assert.Equal(DateTimeKind.Utc, department.DeletedAt.Value.Kind);
            Assert.InRange(department.DeletedAt.Value, deletedFrom, deletedTo);

            Assert.Equal(2, department.DepartmentLocations.Count);
            Assert.Equal(2, department.DepartmentPositions.Count);

            Assert.False(exclusiveLocation.IsActive);
            Assert.NotNull(exclusiveLocation.DeletedAt);
            Assert.True(sharedLocation.IsActive);
            Assert.Null(sharedLocation.DeletedAt);

            Assert.False(exclusivePosition.IsActive);
            Assert.NotNull(exclusivePosition.DeletedAt);
            Assert.True(sharedPosition.IsActive);
            Assert.Null(sharedPosition.DeletedAt);
        });
    }

    [Fact]
    public async Task SoftDelete_WithChildDepartments_KeepsChildrenActiveAndUpdatesDescendantPaths()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var locationId = await CreateLocation();

        var rootId = await CreateDepartment([locationId.Value], cancellationToken);
        var childId = await CreateDepartment(rootId, [locationId.Value], cancellationToken);
        var grandChildId = await CreateDepartment(childId, [locationId.Value], cancellationToken);

        var originalPaths = await ExecuteInDb(async dbContext =>
        {
            var child = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(childId), cancellationToken);

            var grandChild = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(grandChildId), cancellationToken);

            return new
            {
                Child = child.Path.Value,
                GrandChild = grandChild.Path.Value,
            };
        });

        // act
        var result = await ExecuteHandler<SoftDeleteDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new SoftDeleteDepartmentCommand(childId);

            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var root = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(rootId), cancellationToken);

            var child = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(childId), cancellationToken);

            var grandChild = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(grandChildId), cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.True(root.IsActive);
            Assert.False(child.IsActive);
            Assert.True(grandChild.IsActive);

            Assert.NotEqual(originalPaths.Child, child.Path.Value);
            Assert.Contains(".deleted_", child.Path.Value);
            Assert.NotEqual(originalPaths.GrandChild, grandChild.Path.Value);
            Assert.StartsWith($"{child.Path.Value}.", grandChild.Path.Value);
            Assert.Equal(child.Depth + 1, grandChild.Depth);
        });
    }

    [Fact]
    public async Task SoftDelete_WithInactiveDepartment_ReturnsFailure()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var locationId = await CreateLocation();
        var departmentId = await CreateDepartment([locationId.Value], cancellationToken);

        var firstResult = await ExecuteHandler<SoftDeleteDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new SoftDeleteDepartmentCommand(departmentId);

            return sut.HandleAsync(command, cancellationToken);
        });

        // act
        var secondResult = await ExecuteHandler<SoftDeleteDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new SoftDeleteDepartmentCommand(departmentId);

            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        Assert.True(firstResult.IsSuccess);
        Assert.True(secondResult.IsFailure);
    }

    [Fact]
    public async Task SoftDelete_WithUnknownDepartment_ReturnsFailure()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        // act
        var result = await ExecuteHandler<SoftDeleteDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new SoftDeleteDepartmentCommand(Guid.CreateVersion7());

            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        Assert.True(result.IsFailure);
    }

    private async Task<Guid> CreateDepartment(Guid[] locationIds, CancellationToken cancellationToken)
    {
        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateRoot(locationIds)
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);

        return result.Value;
    }

    private async Task<Guid> CreateDepartment(
        Guid parentId,
        Guid[] locationIds,
        CancellationToken cancellationToken)
    {
        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateChild(parentId, locationIds)
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);

        return result.Value;
    }

    private async Task<Guid> CreatePosition(Guid[] departmentIds, CancellationToken cancellationToken)
    {
        var result = await ExecuteHandler<CreatePositionHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreatePositionCommandFakers
                .Create(departmentIds)
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        Assert.True(result.IsSuccess);

        return result.Value;
    }
}
