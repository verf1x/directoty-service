using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Database;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Locations;

public sealed class LocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<LocationsRepository> _logger;

    public LocationsRepository(
        DirectoryServiceDbContext dbContext,
        IDbConnectionFactory dbConnectionFactory,
        ILogger<LocationsRepository> logger)
    {
        _dbContext = dbContext;
        _dbConnectionFactory = dbConnectionFactory;
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

    public async Task<bool> LocationWithNameExistsAsync(
        LocationName locationName,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """
                             SELECT EXISTS(
                                 SELECT 1 FROM locations
                                 WHERE name = @Name
                             )
                             """;

        return await connection.ExecuteScalarAsync<bool>(query, new { Name = locationName.Value });
    }

    public async Task<bool> LocationOnAddressExistsAsync(
        Address address,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """
                             SELECT EXISTS(
                                 SELECT 1 FROM locations
                                 WHERE postal_code = @PostalCode
                                 AND region = @Region
                                 AND city = @City
                                 AND district IS NOT DISTINCT FROM @District
                                 AND street = @Street
                                 AND house = @House
                                 AND building IS NOT DISTINCT FROM @Building
                                 AND apartment IS NOT DISTINCT FROM @Apartment
                             )
                             """;

        var parameters = new
        {
            address.PostalCode,
            address.Region,
            address.City,
            address.District,
            address.Street,
            address.House,
            address.Building,
            address.Apartment,
        };

        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }

    public async Task<bool> LocationExistsByIdAsync(
        Guid locationId,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """
                             SELECT EXISTS(
                                 SELECT 1 FROM locations
                                 WHERE id = @Id
                             )
                             """;

        return await connection.ExecuteScalarAsync<bool>(query, new { Id = locationId });
    }
}
