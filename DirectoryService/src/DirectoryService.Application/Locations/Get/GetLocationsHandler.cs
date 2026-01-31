using System.Text;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Extensions;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Shared;
using DirectoryService.Infrastructure.Postgres.Database;
using FluentValidation;

namespace DirectoryService.Application.Locations.Get;

public class GetLocationsHandler : IQueryHandler<GetLocationsQuery, GetLocationsResponse>
{
    private const string BaseSql = """
                                   SELECT id,
                                          name,
                                          is_active AS isActive,
                                          postal_code AS postalCode,
                                          region,
                                          city,
                                          district,
                                          street,
                                          house,
                                          building,
                                          apartment,
                                          time_zone AS timeZone,
                                          created_at AS createdAt,
                                          count(*) OVER () AS totalCount
                                   FROM locations
                                   """;

    private readonly IValidator<GetLocationsQuery> _validator;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetLocationsHandler(
        IValidator<GetLocationsQuery> validator,
        IDbConnectionFactory dbConnectionFactory)
    {
        _validator = validator;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<GetLocationsResponse, ErrorList>> HandleAsync(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var parameters = new DynamicParameters();
        string sql = BuildSqlQuery(query, parameters);

        using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        long? totalCount = null;

        var locations = await dbConnection.QueryAsync<LocationDto, long, LocationDto>(
            sql,
            map: (location, count) =>
            {
                totalCount ??= count;
                return location;
            },
            splitOn: "totalCount",
            param: parameters);

        return new GetLocationsResponse(locations.ToList(), totalCount ?? 0);
    }

    private string BuildSqlQuery(GetLocationsQuery query, DynamicParameters parameters)
    {
        var sqlBuilder = new StringBuilder(BaseSql);

        ApplyFilters(sqlBuilder, parameters, query);

        sqlBuilder.ApplySorting(query.SortBy, query.SortDirection);
        sqlBuilder.ApplyPagination(parameters, query.Pagination.PageNumber, query.Pagination.PageSize);

        return sqlBuilder.ToString();
    }

    private void ApplyFilters(StringBuilder sqlBuilder, DynamicParameters parameters, GetLocationsQuery query)
    {
        var whereClauses = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            whereClauses.Add("""
                             (LOWER(name) ILIKE LOWER(@Search) OR 
                              LOWER(region) ILIKE LOWER(@Search) OR 
                              LOWER(city) ILIKE LOWER(@Search) OR 
                              LOWER(district) ILIKE LOWER(@Search) OR 
                              LOWER(street) ILIKE LOWER(@Search))
                             """);
            parameters.Add("Search", $"%{query.Search}%");
        }

        AddFilter(whereClauses, parameters, "postal_code = @PostalCode", "PostalCode", query.PostalCode);
        AddILikeFilter(whereClauses, parameters, "region", "Region", query.Region);
        AddILikeFilter(whereClauses, parameters, "city", "City", query.City);
        AddILikeFilter(whereClauses, parameters, "district", "District", query.District);
        AddILikeFilter(whereClauses, parameters, "street", "Street", query.Street);
        AddILikeFilter(whereClauses, parameters, "house", "House", query.House);
        AddILikeFilter(whereClauses, parameters, "building", "Building", query.Building);
        AddILikeFilter(whereClauses, parameters, "apartment", "Apartment", query.Apartment);
        AddILikeFilter(whereClauses, parameters, "time_zone", "TimeZone", query.TimeZone);

        if (whereClauses.Count > 0)
        {
            sqlBuilder.Append("\nWHERE ");
            sqlBuilder.Append(string.Join("\nAND ", whereClauses));
        }
    }

    private void AddFilter(
        List<string> clauses,
        DynamicParameters parameters,
        string sqlCondition,
        string paramName,
        string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            clauses.Add(sqlCondition);
            parameters.Add(paramName, value);
        }
    }

    private void AddILikeFilter(
        List<string> clauses,
        DynamicParameters parameters,
        string columnName,
        string paramName,
        string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            clauses.Add($"{columnName} ILIKE @{paramName}");
            parameters.Add(paramName, $"%{value}%");
        }
    }
}