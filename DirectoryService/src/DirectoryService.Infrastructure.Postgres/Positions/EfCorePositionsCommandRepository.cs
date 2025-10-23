using CSharpFunctionalExtensions;
using DirectoryService.Application.Positions;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Positions;

public class EfCorePositionsCommandRepository : IPositionsCommandRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<EfCorePositionsCommandRepository> _logger;

    public EfCorePositionsCommandRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<EfCorePositionsCommandRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Positions.AddAsync(position, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return position.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error adding position with ID {PositionId}  to the database.",
                position.Id.Value);

            return Error.Failure(
                "position.insert",
                "An error occurred while adding the position to the database.");
        }
    }
}