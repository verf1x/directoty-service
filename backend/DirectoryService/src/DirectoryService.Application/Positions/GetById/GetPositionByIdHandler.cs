using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Positions;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Positions.GetById;

public class GetPositionByIdHandler : IQueryHandler<GetPositionByIdQuery, GetPositionResponse>
{
    private readonly IValidator<GetPositionByIdQuery> _validator;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetPositionByIdHandler(
        IValidator<GetPositionByIdQuery> validator,
        IDbConnectionFactory dbConnectionFactory)
    {
        _validator = validator;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<GetPositionResponse, ErrorList>> HandleAsync(
        GetPositionByIdQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var position = await connection.QueryFirstOrDefaultAsync<GetPositionResponse>(
            """
            SELECT id,
                   name,
                   description,
                   created_at,
                   updated_at
            FROM positions
            WHERE id = @Id
              AND is_active = true
              AND deleted_at IS NULL
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