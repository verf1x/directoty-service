using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Domain;
using DirectoryService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public sealed class EfCoreLocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<EfCoreLocationsRepository> _logger;

    public EfCoreLocationsRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<EfCoreLocationsRepository> logger)
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