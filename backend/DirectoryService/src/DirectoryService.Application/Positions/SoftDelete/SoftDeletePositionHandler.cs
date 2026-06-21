using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Positions.SoftDelete;

public class SoftDeletePositionHandler(
    IPositionsRepository positionsRepository,
    IDbConnectionFactory dbConnectionFactory,
    ITransactionManager transactionManager)
    : ICommandHandler<SoftDeletePositionCommand, Guid>
{
    public async Task<Result<Guid, ErrorList>> HandleAsync(
        SoftDeletePositionCommand command,
        CancellationToken cancellationToken)
    {
        var positionResult = await positionsRepository.GetByAsync(p => p.Id == command.Id, cancellationToken);

        if (positionResult.IsFailure)
        {
            return positionResult.Error.ToErrors();
        }

        var position = positionResult.Value;

        var activeDepartmentsWithPostionExists = await ActiveDepartmentsWithPostionExists(position, cancellationToken);

        if (activeDepartmentsWithPostionExists)
        {
            return Error.Failure(
                    "position.delete",
                    "Cannot delete the position because there are active departments associated with it.")
                .ToErrors();
        }

        position.SoftDelete();

        var saveChangesResult = await transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            return saveChangesResult.Error.ToErrors();
        }

        return position.Id.Value;
    }

    private async Task<bool> ActiveDepartmentsWithPostionExists(Position position, CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<bool>(
            """
            SELECT EXISTS(
                SELECT 1
                FROM departments_positions dp
                JOIN departments d ON dp.department_id = d.id
                WHERE dp.position_id = @PositionId 
                AND d.is_active = true
                AND dp.deleted_at IS NULL
            )
            """,
            new { PositionId = position.Id.Value });
    }
}