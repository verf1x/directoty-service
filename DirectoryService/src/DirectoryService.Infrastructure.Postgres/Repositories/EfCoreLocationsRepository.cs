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

    public async Task<Result<bool, Error>> CheckIfLocationWithNameExistsAsync(
        LocationName locationName,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Locations
                .AnyAsync(l => l.Name == locationName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error checking if location with name {LocationName} exists in the database",
                locationName.Value);

            return Error.Failure(
                "location.check.name",
                "An error occurred while checking for the location in the database.");
        }
    }

    public async Task<Result<bool, Error>> CheckIfLocationOnAddressExistsAsync(
        Address address,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Locations
                .AnyAsync(l => l.Address == address, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error checking if location with address {Address} exists in the database",
                address);

            return Error.Failure(
                "location.check.address",
                "An error occurred while checking for the location in the database.");
        }
    }
}