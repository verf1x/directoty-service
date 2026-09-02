using System.Net;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Locations;

public class DeleteLocationTests(DirectoryServiceTestsWebFactory webFactory) : BaseIntegrationTest(webFactory)
{
    [Fact]
    public async Task DeleteLocation_WithValidData_ShouldRemainInDatabaseAndReturn404FromGet()
    {
        // arrange
        CancellationToken cancellationToken = CancellationToken.None;

        using HttpClient client = webFactory.CreateClient();

        // act
        Result<Guid, ErrorList> locationIdResult =
            await ExecuteHandler<CreateLocationHandler, Result<Guid, ErrorList>>(async sut => await sut.HandleAsync(
                CreateLocationCommandFakers.Create().Generate(),
                cancellationToken));

        var deleteResponse = await client.DeleteAsync(
            $"/api/locations/{locationIdResult.Value}",
            cancellationToken);

        var getResponse = await client.GetAsync(
            $"/api/locations/{locationIdResult.Value}",
            cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        await ExecuteInDb(async dbContext =>
        {
            var location = await dbContext.Locations
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(
                    l => l.Id == LocationId.Create(locationIdResult.Value),
                    cancellationToken);

            Assert.NotNull(location);
            Assert.False(location.IsActive);
            Assert.True(location.DeletedAt.HasValue);
        });
    }
}