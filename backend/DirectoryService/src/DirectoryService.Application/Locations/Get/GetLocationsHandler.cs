using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Locations.Get;

public class GetLocationsHandler(
    IValidator<GetLocationsQuery> validator,
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetLocationsQuery, PagedResult<LocationListItemDto>>
{
    public async Task<Result<PagedResult<LocationListItemDto>, ErrorList>> HandleAsync(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var parameters = new DynamicParameters();
        parameters.Add(
            "search",
            string.IsNullOrWhiteSpace(query.Search) ? null : $"%{query.Search}%",
        DbType.String);
        parameters.Add("min_departments_count", query.MinDepartmentsCount, DbType.Int32);
        parameters.Add("limit", query.Pagination.PageSize, DbType.Int32);
        parameters.Add("offset", (query.Pagination.Page - 1) * query.Pagination.PageSize, DbType.Int32);

        var sortBy = query.SortBy.ToLowerInvariant() switch
        {
            "name" => "name",
            "createdat" => "created_at",
            "departmentscount" => "departments_count",
            _ => "name"
        };

        var sortDirection = string.Equals(query.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase)
            ? "DESC"
            : "ASC";

        var orderByClause = $"ORDER BY {sortBy} {sortDirection}";


        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);


        var rows = (await connection.QueryAsync<LocationListRow>(
            $"""
            WITH locations_departments_count AS (SELECT l.id,
                                            l.name,
                                            l.postal_code,
                                            l.region,
                                            l.city,
                                            l.district,
                                            l.street,
                                            l.house,
                                            l.building,
                                            l.apartment,
                                            l.time_zone,
                                            l.created_at,
                                            COUNT(dl.department_id) AS departments_count
                                     FROM locations l
                                              LEFT JOIN department_locations dl ON l.id = dl.location_id
                                     WHERE (@search IS NULL OR l.name ILIKE @search)
                                     GROUP BY l.id)

            SELECT 
                id,
                name,
                postal_code,
                region,
                city,
                district,
                street,
                house,
                building,
                apartment,
                time_zone,
                created_at,
                departments_count,
                COUNT(*) OVER () AS total_count
            FROM locations_departments_count
            WHERE (@min_departments_count IS NULL OR departments_count >= @min_departments_count)
            {orderByClause}
            LIMIT @limit OFFSET @offset
            """,
            param: parameters)).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var responseItems = rows.ConvertAll(row => new LocationListItemDto
        {
            Id = row.Id,
            Name = row.Name,
            PostalCode = row.PostalCode,
            Region = row.Region,
            City = row.City,
            District = row.District,
            Street = row.Street,
            House = row.House,
            Building = row.Building,
            Apartment = row.Apartment,
            TimeZone = row.TimeZone,
            CreatedAt = row.CreatedAt,
            DepartmentsCount = row.DepartmentsCount,
        });

        return new PagedResult<LocationListItemDto>(
            responseItems,
            query.Pagination.Page,
            query.Pagination.PageSize,
            totalCount);
    }
}
