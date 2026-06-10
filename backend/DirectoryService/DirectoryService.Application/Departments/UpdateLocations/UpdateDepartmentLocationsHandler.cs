using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Departments.UpdateLocations;

public class UpdateDepartmentLocationsHandler : ICommandHandler<UpdateDepartmentLocationsCommand, Guid>
{
    private readonly IValidator<UpdateDepartmentLocationsCommand> _validator;
    private readonly ITransactionManager _transactionManager;
    private readonly IDepartmentsRepository _departmentsRepository;

    public UpdateDepartmentLocationsHandler(
        IValidator<UpdateDepartmentLocationsCommand> validator,
        ITransactionManager transactionManager,
        IDepartmentsRepository departmentsRepository)
    {
        _validator = validator;
        _transactionManager = transactionManager;
        _departmentsRepository = departmentsRepository;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateDepartmentLocationsCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var departmentId = DepartmentId.Create(command.DepartmentId);

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        await using var transactionScope = transactionScopeResult.Value;

        bool departmentActive = await _departmentsRepository.DepartmentActiveByIdAsync(
            command.DepartmentId,
            cancellationToken);

        if (!departmentActive)
        {
            await transactionScope.RollbackAsync(cancellationToken);
            return Errors.General.NotFound(departmentId.Value).ToErrors();
        }

        var departmentResult = await _departmentsRepository.GetByIdWithLocationsAsync(departmentId, cancellationToken);
        if (departmentResult.IsFailure)
        {
            await transactionScope.RollbackAsync(cancellationToken);
            return departmentResult.Error.ToErrors();
        }

        var newLocations = command.LocationIds.Select(LocationId.Create).ToList();

        bool newLocationsActive = await _departmentsRepository.LocationsActiveByIdsAsync(
            newLocations,
            cancellationToken);

        if (!newLocationsActive)
        {
            await transactionScope.RollbackAsync(cancellationToken);
            return Errors.General.NotFound().ToErrors();
        }

        var departmentLocations = command.LocationIds.Select(id => new DepartmentLocation(
                DepartmentId.Create(command.DepartmentId),
                LocationId.Create(id)))
            .ToList();

        var updateResult = departmentResult.Value.UpdateLocations(departmentLocations);
        if (updateResult.IsFailure)
        {
            await transactionScope.RollbackAsync(cancellationToken);
            return updateResult.Error.ToErrors();
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            await transactionScope.RollbackAsync(cancellationToken);
            return saveResult.Error.ToErrors();
        }

        var commitResult = await transactionScope.CommitAsync(cancellationToken);
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();

        return departmentId.Value;
    }
}