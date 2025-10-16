using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Result<Location, Error>> GetByNameAsync(
        LocationName locationName,
        CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(l => l.Name == locationName, cancellationToken);

        if (location is null)
            return Errors.General.NotFound();

        return location;
    }

    public async Task<Result<Location, Error>> GetByAddressAsync(Address address, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(l => l.Address == address, cancellationToken);

        if (location is null)
            return Errors.General.NotFound();

        return location;
    }
}