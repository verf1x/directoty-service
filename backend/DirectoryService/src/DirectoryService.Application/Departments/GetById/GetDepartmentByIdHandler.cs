using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Departments.GetById;

public class GetDepartmentByIdHandler : IQueryHandler<GetDepartmentByIdQuery, GetDepartmentResponse>
{
    private readonly IValidator<GetDepartmentByIdQuery> _validator;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetDepartmentByIdHandler(
        IValidator<GetDepartmentByIdQuery> validator,
        IDbConnectionFactory dbConnectionFactory)
    {
        _validator = validator;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<GetDepartmentResponse, ErrorList>> HandleAsync(
        GetDepartmentByIdQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var department = await connection.QueryFirstOrDefaultAsync<GetDepartmentResponse>(
            """
            SELECT id,
                   name,
                   identifier,
                   path,
                   created_at,
                   updated_at
            FROM departments
            WHERE id = @Id
              AND is_active = true
              AND deleted_at IS NULL
            LIMIT 1;
            """,
            new { query.Id });

        if (department == null)
        {
            return Error.NotFound("department.not_found", "Department not found.").ToErrors();
        }

        return department;
    }
}