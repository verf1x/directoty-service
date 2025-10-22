using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Locations;

public sealed class EfCoreLocationsCommandRepository : ILocationsCommandRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<EfCoreLocationsCommandRepository> _logger;

    public EfCoreLocationsCommandRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<EfCoreLocationsCommandRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Locations.AddAsync(location, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return location.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error adding location with ID {LocationId} to the database",
                location.Id.Value);

            return Error.Failure(
                "location.insert",
                "An error occurred while adding the location to the database.");
        }
    }
}