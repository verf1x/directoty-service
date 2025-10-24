using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Departments;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;
using DirectoryService.Infrastructure.Postgres.Database;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Departments;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceWriteDbContext _writeDbContext;
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<DepartmentsRepository> _logger;

    public DepartmentsRepository(
        DirectoryServiceWriteDbContext writeDbContext,
        IDbConnectionFactory dbConnectionFactory,
        ILogger<DepartmentsRepository> logger)
    {
        _writeDbContext = writeDbContext;
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _writeDbContext.Departments.AddAsync(department, cancellationToken);

            await _writeDbContext.SaveChangesAsync(cancellationToken);

            return department.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error adding department with ID {DepartmentId} to the database",
                department.Id.Value);

            return Error.Failure(
                "department.insert",
                "An error occurred while adding the department to the database.");
        }
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
                             SELECT EXISTS(
                                SELECT 1 FROM Departments 
                                WHERE identifier = @Identifier);
                             """;

        var parameters = new { Identifier = identifier };

        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }

    public async Task<bool> DepartmentActiveByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """
                             SELECT EXISTS(
                                SELECT 1 FROM Departments
                                WHERE "Id" = @Id
                                AND is_active = true
                             )
                             """;

        return await connection.ExecuteScalarAsync<bool>(query, new { Id = id });
    }
}