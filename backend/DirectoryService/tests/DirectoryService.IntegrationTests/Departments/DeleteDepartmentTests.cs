using System.Net;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class DeleteDepartmentTests(DirectoryServiceTestsWebFactory webFactory) : BaseIntegrationTest(webFactory)
{
    [Fact]
    public async Task DeleteDepartment_WithValidData_ShouldRemainInDatabaseAndReturn404FromGet()
    {
        // arrange
        LocationId locationId = await CreateLocation();

        CancellationToken cancellationToken = CancellationToken.None;

        using var client = webFactory.CreateClient();

        // act
        Result<Guid, ErrorList> departmentIdResult =
            await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(async sut => await sut.HandleAsync(
                CreateDepartmentCommandFaker.CreateRoot([locationId.Value]).Generate(),
                cancellationToken));

        var deleteResponse = await client.DeleteAsync(
            $"/api/departments/{departmentIdResult.Value}",
            cancellationToken);

        var getResponse = await client.GetAsync(
            $"/api/departments/{departmentIdResult.Value}",
            cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(
                    d => d.Id == DepartmentId.Create(departmentIdResult.Value),
                    cancellationToken);

            Assert.NotNull(department);
            Assert.False(department.IsActive);
            Assert.True(department.DeletedAt.HasValue);
        });
    }
}
