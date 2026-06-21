using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Positions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Departments.SoftDelete;

public class SoftDeleteDepartmentHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager) : ICommandHandler<SoftDeleteDepartmentCommand, Guid>
{
    public async Task<Result<Guid, ErrorList>> HandleAsync(
        SoftDeleteDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var departmentResult = await departmentsRepository.GetByAsync(d => d.Id == command.Id, cancellationToken);

        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrors();
        }

        var department = departmentResult.Value;

        if (!department.IsActive)
        {
            return department.Id.Value;
        }

        department.SoftDelete();

        await DeactivateOrphanLocationsAsync(department.Id, cancellationToken);
        await DeactivateOrphanPositionsAsync(department.Id, cancellationToken);

        var saveChangesResult = await transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            return saveChangesResult.Error.ToErrors();
        }

        return department.Id.Value;
    }

    private async Task DeactivateOrphanLocationsAsync(DepartmentId departmentId, CancellationToken cancellationToken)
    {
        var locations = await locationsRepository.GetByDepartmentIdToDeactivateAsync(departmentId, cancellationToken);

        foreach (var location in locations)
        {
            location.SoftDelete();
        }
    }

    private async Task DeactivateOrphanPositionsAsync(DepartmentId departmentId, CancellationToken cancellationToken)
    {
        var positions = await positionsRepository.GetByDepartmentIdToDeactivateAsync(departmentId, cancellationToken);

        foreach (var position in positions)
        {
            position.SoftDelete();
        }
    }
}