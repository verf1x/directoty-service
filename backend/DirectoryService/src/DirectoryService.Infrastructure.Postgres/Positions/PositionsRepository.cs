using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Database;
using DirectoryService.Application.Positions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Positions;

public class PositionsRepository : IPositionsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<PositionsRepository> _logger;

    public PositionsRepository(
        DirectoryServiceDbContext dbContext,
        IDbConnectionFactory dbConnectionFactory,
        ILogger<PositionsRepository> logger)
    {
        _dbContext = dbContext;
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Positions.AddAsync(position, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return position.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error adding position with ID {PositionId}  to the database.",
                position.Id.Value);

            return Error.Failure(
                "position.insert",
                "An error occurred while adding the position to the database.");
        }
    }

    public async Task<bool> IsPositionWithNameAlreadyActive(
        string positionName,
        CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """
                             SELECT EXISTS(
                                SELECT 1 FROM positions
                                WHERE is_active = true
                                AND name = @PositionName
                             )
                             """;

        return await connection.ExecuteScalarAsync<bool>(query, new { PositionName = positionName });
    }

    public async Task<Result<Position, Error>> GetByAsync(
        Expression<Func<Position, bool>> predicate,
        CancellationToken cancellationToken)
    {
        var position = await _dbContext.Positions.FirstOrDefaultAsync(predicate, cancellationToken);

        if (position is null)
            return Errors.General.NotFound();

        return position;
    }

    public async Task<IReadOnlyList<Position>> GetByDepartmentIdToDeactivateAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        var positionsToDeactivate = await _dbContext.Positions
            .Where(p => p.IsActive)
            .Where(p => p.DepartmentPositions.Any(dp => dp.DepartmentId == departmentId))
            .Where(p => !p.DepartmentPositions.Any(dp =>
                dp.DepartmentId != departmentId &&
                dp.Department.IsActive))
            .ToListAsync(cancellationToken);

        return positionsToDeactivate;
    }
}