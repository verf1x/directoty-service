using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Locations.GetById;

public class GetLocationByIdHandler : IQueryHandler<GetLocationByIdQuery, GetLocationResponse>
{
    private readonly IValidator<GetLocationByIdQuery> _validator;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetLocationByIdHandler(
        IValidator<GetLocationByIdQuery> validator,
        IDbConnectionFactory dbConnectionFactory)
    {
        _validator = validator;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<GetLocationResponse, ErrorList>> HandleAsync(
        GetLocationByIdQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var location = await connection.QueryFirstOrDefaultAsync<GetLocationResponse>(
            """
            SELECT id,
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
                   updated_at
            FROM locations
            WHERE id = @Id
              AND is_active = true
              AND deleted_at IS NULL
            LIMIT 1;
            """,
            new { query.Id });

        if (location == null)
        {
            return Error.NotFound("location.not_found", "Location not found.").ToErrors();
        }

        return location;
    }
}