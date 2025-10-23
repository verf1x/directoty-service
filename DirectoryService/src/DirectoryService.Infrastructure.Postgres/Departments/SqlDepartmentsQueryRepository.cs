using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Departments.Dtos;
using DirectoryService.Domain.Shared;
using DirectoryService.Infrastructure.Postgres.Database;

namespace DirectoryService.Infrastructure.Postgres.Departments;

public class SqlDepartmentsQueryRepository : IDepartmentsQueryRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SqlDepartmentsQueryRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<DepartmentParentDto, Error>> GetDepartmentParentAsync(
        Guid parentId,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """
                             SELECT "Id", Path, Depth FROM Departments
                             WHERE "Id" = @ParentId
                             """;

        var parameters = new { ParentId = parentId };

        var result = await connection.QuerySingleOrDefaultAsync<DepartmentParentDto>(query, parameters);

        if (result is null)
            return Errors.General.NotFound();

        return result;
    }

    public async Task<bool> DepartmentWithIdentifierExistsAsync(
        string identifier,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """
                             SELECT EXISTS (
                                SELECT 1 FROM Departments 
                                WHERE identifier = @Identifier);
                             """;

        var parameters = new { Identifier = identifier };

        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }
}