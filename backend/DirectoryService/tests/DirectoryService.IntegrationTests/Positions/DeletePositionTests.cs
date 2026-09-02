using System.Net;
using DirectoryService.Domain.Positions;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Positions;

public class DeletePositionsTests(DirectoryServiceTestsWebFactory webFactory) : BaseIntegrationTest(webFactory)
{
    [Fact]
    public async Task DeletePosition_WithValidData_ShouldRemainInDatabaseAndReturn404FromGet()
    {
        // arrange
        CancellationToken cancellationToken = CancellationToken.None;

        using HttpClient client = webFactory.CreateClient();

        PositionId positionId = await ExecuteInDb(async dbContext =>
        {
            var position = new Position(
                PositionId.CreateNew(),
                PositionName.Create($"Position {Guid.CreateVersion7()}").Value,
                null,
                []);

            dbContext.Positions.Add(position);
            await dbContext.SaveChangesAsync(cancellationToken);

            return position.Id;
        });

        // act
        var deleteResponse = await client.DeleteAsync(
            $"/api/positions/{positionId.Value}",
            cancellationToken);

        var getResponse = await client.GetAsync(
            $"/api/positions/{positionId.Value}",
            cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        await ExecuteInDb(async dbContext =>
        {
            var position = await dbContext.Positions
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(
                    p => p.Id == positionId,
                    cancellationToken);

            Assert.NotNull(position);
            Assert.False(position.IsActive);
            Assert.True(position.DeletedAt.HasValue);
        });
    }
}
