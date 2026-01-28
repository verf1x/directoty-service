using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Departments.UpdateParent;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentParentTests : BaseIntegrationTest
{
    public UpdateDepartmentParentTests(DirectoryServiceTestsWebFactory webFactory)
        : base(webFactory)
    {
    }

    [Fact]
    public async Task UpdateParent_WithValidParent_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        var departmentsIds = await CreateDepartmentsArray(cancellationToken);

        // act
        var result = await ExecuteHandler<UpdateDepartmentParentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new UpdateDepartmentParentCommand(
                departmentsIds[2],
                departmentsIds[1]);
            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department3 = await dbContext.Departments
                .Include(d => d.ParentId)
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentsIds[2]), cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            Assert.Equal(DepartmentId.Create(departmentsIds[1]), department3.ParentId);
        });
    }

    [Fact]
    public async Task UpdateParent_ToRemoveParent_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        var departmentsIds = await CreateDepartmentsArray(cancellationToken);

        // act
        var result = await ExecuteHandler<UpdateDepartmentParentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new UpdateDepartmentParentCommand(
                departmentsIds[2],
                null);
            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department3 = await dbContext.Departments
                .Include(d => d.ParentId)
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentsIds[2]), cancellationToken);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
            Assert.Null(department3.ParentId);
        });
    }

    [Fact]
    public async Task UpdateParent_ToDescendant_ShouldFail()
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        var departmentsIds = await CreateDepartmentsArray(cancellationToken);

        // act
        var result = await ExecuteHandler<UpdateDepartmentParentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new UpdateDepartmentParentCommand(
                departmentsIds[2],
                departmentsIds[3]);
            return sut.HandleAsync(command, cancellationToken);
        });

        // assert
        await ExecuteInDb(async dbContext =>
        {
            var department3 = await dbContext.Departments
                .Include(d => d.ParentId)
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentsIds[2]), cancellationToken);

            Assert.NotNull(department3);
            Assert.True(result.IsFailure);
            Assert.NotEqual(department3.ParentId, departmentsIds[3]);
            Assert.Equal(departmentsIds[0], department3.ParentId);
        });
    }

    private async Task<Guid[]> CreateDepartmentsArray(CancellationToken cancellationToken)
    {
        var locationId = await CreateLocation();

        var rootDepartment1Id = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateRoot([locationId.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        var department2Id = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateChild(rootDepartment1Id.Value, [locationId.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        var department3Id = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateChild(rootDepartment1Id.Value, [locationId.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        var department4Id = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateChild(department3Id.Value, [locationId.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        var department5Id = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateChild(department3Id.Value, [locationId.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        return
        [
            rootDepartment1Id.Value,
            department2Id.Value,
            department3Id.Value,
            department4Id.Value,
            department5Id.Value
        ];
    }
}