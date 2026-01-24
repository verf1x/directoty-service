using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Departments;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using DirectoryService.Infrastructure.Postgres.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Path = DirectoryService.Domain.Departments.Path;

namespace DirectoryService.Infrastructure.Postgres.Departments;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<DepartmentsRepository> _logger;

    public DepartmentsRepository(
        DirectoryServiceDbContext dbContext,
        IDbConnectionFactory dbConnectionFactory,
        ILogger<DepartmentsRepository> logger)
    {
        _dbContext = dbContext;
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task<Result<Department, Error>> GetByIdWithLocationsAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Include(d => d.DepartmentLocations)
            .FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken);

        if (department is null)
        {
            return Error.NotFound(
                "department.not.found",
                $"Department with ID {departmentId.Value} was not found.");
        }

        return department;
    }

    public async Task<Result<Department, Error>> GetByIdWithLockAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        var id = departmentId.Value;

        var department = await _dbContext.Departments.FromSql(
                $"SELECT * FROM departments WHERE id = {id} FOR UPDATE")
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            return Error.NotFound(
                "department.not.found",
                $"Department with ID {departmentId.Value} was not found.");
        }

        return department;
    }

    public async Task<UnitResult<Error>> LockDescendants(Path path, CancellationToken cancellationToken)
    {
        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
             SELECT * FROM departments
             WHERE path <@ {path.Value}::ltree 
             AND path != {path.Value}::ltree 
             FOR UPDATE
             """,
            cancellationToken);

        return UnitResult.Success<Error>();
    }

    public async Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

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
                             SELECT id, path, depth FROM departments
                             WHERE id = @ParentId
                             """;

        var parameters = new { ParentId = parentId };

        var result = await connection.QuerySingleOrDefaultAsync<DepartmentParentDto>(query, parameters);

        if (result is null)
            return Errors.General.NotFound();

        return result;
    }

    public async Task<bool> DepartmentWithIdentifierExistAsync(
        string identifier,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """
                             SELECT EXISTS(
                                SELECT 1 FROM departments 
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
                                SELECT 1 FROM departments
                                WHERE id = @Id
                                AND is_active = true
                             )
                             """;

        return await connection.ExecuteScalarAsync<bool>(query, new { Id = id });
    }

    public async Task DeleteLocationsByDepartmentIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        await _dbContext.Locations
            .SelectMany(l => l.DepartmentLocations)
            .Where(dl => dl.DepartmentId == departmentId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<bool> LocationsExistByIdsAsync(
        IEnumerable<LocationId> locationIds,
        CancellationToken cancellationToken)
    {
        var locationIdsList = locationIds.Select(id => id.Value).ToList();

        var connection = _dbContext.Database.GetDbConnection();

        const string sql = """
                           SELECT COUNT(*)
                           FROM locations
                           WHERE id = ANY(@LocationIds)
                           """;

        int existingCount = await connection.ExecuteScalarAsync<int>(
            sql,
            new { LocationIds = locationIdsList });

        return existingCount == locationIdsList.Count;
    }

    public async Task<bool> LocationsActiveByIdsAsync(
        IEnumerable<LocationId> locationIds,
        CancellationToken cancellationToken)
    {
        var locationIdsList = locationIds.Select(id => id.Value).ToList();

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string sql = """
                           SELECT COUNT(*)
                           FROM locations
                           WHERE id = ANY(@LocationIds)
                           AND is_active = true
                           """;

        int activeCount = await connection.ExecuteScalarAsync<int>(
            sql,
            new { LocationIds = locationIdsList });

        return activeCount == locationIdsList.Count;
    }

    public async Task<bool> IsDescendantAsync(
        DepartmentId currentId,
        DepartmentId newParentId)
    {
        var dbConnection = _dbContext.Database.GetDbConnection();

        bool hasDescendant = await dbConnection.ExecuteScalarAsync<bool>(
            """
            SELECT EXISTS (
                SELECT 1 
                FROM departments AS currentDepartment
                JOIN departments AS newParent ON newParent.id = @NewParentId
                WHERE currentDepartment.id = @CurrentId
                  AND newParent.path <@ currentDepartment.path
            );
            """,
            new { CurrentId = currentId.Value, NewParentId = newParentId.Value, });

        return hasDescendant;
    }

    public async Task<UnitResult<ErrorList>> UpdateDepartmentsHierarchyAsync(Department department, short oldDepth, Path oldPath)
    {
        var dbConnection = _dbContext.Database.GetDbConnection();

        try
        {
            await dbConnection.ExecuteAsync(
                """
                UPDATE departments
                SET path = @NewPath::ltree || 
                    CASE 
                        WHEN nlevel(path) > nlevel(@OldPath::ltree) 
                        THEN subpath(path, nlevel(@OldPath::ltree))
                        ELSE ''::ltree 
                    END,
                    
                    depth = depth - @DepthDifference
                WHERE path <@ @OldPath::ltree
                """,
                new
                {
                    OldPath = oldPath.Value,
                    NewPath = department.Path.Value,
                    DepthDifference = (short)(oldDepth - department.Depth),
                });
        }
        catch (Exception ex)
        {
            return Error.Failure(
                    "update.departments.hierarchy.failed",
                    "Failed to update hierarchy for department with Id {department.Id.Value}")
                .ToErrors();
        }

        return UnitResult.Success<ErrorList>();
    }
}