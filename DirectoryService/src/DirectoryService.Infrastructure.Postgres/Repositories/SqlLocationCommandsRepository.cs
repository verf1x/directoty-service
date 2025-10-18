using Dapper;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Infrastructure.Postgres.Database;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class SqlLocationQueriesRepository : ILocationQueriesRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SqlLocationQueriesRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CheckIfLocationWithNameExistsAsync(
        LocationName locationName,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = "SELECT COUNT(1) FROM Locations WHERE Name = @Name";

        var sqlParams = new { Name = locationName.Value };

        int count = await connection.ExecuteScalarAsync<int>(query, sqlParams);

        return count > 0;
    }

    public async Task<bool> CheckIfLocationOnAddressExistsAsync(
        Address address,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """

                             SELECT COUNT(1) FROM Locations
                             WHERE "postal_code" = @PostalCode
                             AND Region = @Region
                             AND City = @City
                             AND District = @District
                             AND Street = @Street
                             AND House = @House
                             AND Building = @Building
                             AND Apartment = @Apartment
                             """;

        var sqlParams = new
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

        int count = await connection.ExecuteScalarAsync<int>(query, sqlParams);

        return count > 0;
    }
}