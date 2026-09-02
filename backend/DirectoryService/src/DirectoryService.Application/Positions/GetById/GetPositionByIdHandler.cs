using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Positions;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Positions.GetById;

public class GetPositionByIdHandler(
    IValidator<GetPositionByIdQuery> validator,
    IReadDbConnectionFactory readDbConnectionFactory)
    : IQueryHandler<GetPositionByIdQuery, GetPositionResponse>
{
    public async Task<Result<GetPositionResponse, ErrorList>> HandleAsync(
        GetPositionByIdQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        using var connection = await readDbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var position = await connection.QueryFirstOrDefaultAsync<GetPositionResponse>(
            """
            SELECT id,
                   name,
                   description,
                   created_at,
                   updated_at
            FROM available.positions
            WHERE id = @Id
            LIMIT 1;
            """,
            new { query.Id });

        if (position == null)
        {
            return Error.NotFound("position.not_found", "Position not found.").ToErrors();
        }

        return position;
    }
}