using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Locations.SoftDelete;

public class SoftDeleteLocationHandler(
    ILocationsRepository locationsRepository,
    IDbConnectionFactory dbConnectionFactory,
    ITransactionManager transactionManager)
    : ICommandHandler<SoftDeleteLocationCommand, Guid>
{
    public async Task<Result<Guid, ErrorList>> HandleAsync(
        SoftDeleteLocationCommand command,
        CancellationToken cancellationToken)
    {
        var locationResult = await locationsRepository.GetByAsync(l => l.Id == command.Id, cancellationToken);

        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrors();
        }

        var location = locationResult.Value;

        var activeDepartmentsWithLocationExists = await ActiveDepartmentsWithLocationExists(
            location,
            cancellationToken);

        if (activeDepartmentsWithLocationExists)
        {
            return Error.Failure(
                    "location.delete",
                    "Cannot delete the location because there are active departments associated with it.")
                .ToErrors();
        }

        location.SoftDelete();

        var saveChangesResult = await transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            return saveChangesResult.Error.ToErrors();
        }

        return location.Id.Value;
    }

    private async Task<bool> ActiveDepartmentsWithLocationExists(
        Location location,
        CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<bool>(
            """
            SELECT EXISTS(
                SELECT 1
                FROM departments_locations dl
                JOIN departments d ON dl.department_id = d.id
                WHERE dl.location_id = @LocationId 
                AND d.is_active
                AND dl.deleted_at IS NULL)
            """,
            new { LocationId = location.Id.Value });
    }
}