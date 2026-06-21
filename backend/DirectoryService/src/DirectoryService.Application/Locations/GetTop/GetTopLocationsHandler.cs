using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Locations.GetTop;

public class GetTopLocationsHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetTopLocationsQuery, GetTopLocationsResponse>
{
    public async Task<Result<GetTopLocationsResponse, ErrorList>> HandleAsync(
        GetTopLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        using var dbConnection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var topLocations = await dbConnection.QueryAsync<TopLocationDto>(
            """
            SELECT l.id                    AS location_id,
                   l.name,
                   l.postal_code,
                   l.region,
                   l.city,
                   l.district,
                   l.street,
                   l.house,
                   l.building,
                   l.apartment,
                   COUNT(dl.department_id) AS departments_count
            FROM locations l
                     LEFT JOIN department_locations dl ON l.id = dl.location_id
            WHERE l.is_active = TRUE
              AND l.deleted_at IS NULL
            GROUP BY l.id
            ORDER BY departments_count DESC
            LIMIT 5
            """,
            cancellationToken);

        return new GetTopLocationsResponse([.. topLocations]);
    }
}
