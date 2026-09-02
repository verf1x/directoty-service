using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Locations.GetById;

public class GetLocationByIdHandler(
    IValidator<GetLocationByIdQuery> validator,
    IReadDbConnectionFactory readDbConnectionFactory) : IQueryHandler<GetLocationByIdQuery, GetLocationResponse>
{
    public async Task<Result<GetLocationResponse, ErrorList>> HandleAsync(
        GetLocationByIdQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        using var connection = await readDbConnectionFactory.CreateConnectionAsync(cancellationToken);

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
            FROM available.locations
            WHERE id = @Id
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