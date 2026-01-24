using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Departments.UpdateParent;

public sealed class UpdateDepartmentParentHandler : ICommandHandler<UpdateDepartmentParentCommand, Guid>
{
    private readonly IValidator<UpdateDepartmentParentCommand> _validator;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;

    public UpdateDepartmentParentHandler(
        IValidator<UpdateDepartmentParentCommand> validator,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager)
    {
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        UpdateDepartmentParentCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var departmentId = DepartmentId.Create(command.DepartmentId);

        var newParentId = command.ParentId.HasValue
            ? DepartmentId.Create(command.ParentId.Value)
            : null;

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        using var transactionScope = transactionScopeResult.Value;

        try
        {
            var updateParentResult = await ChangeParentAsync(departmentId, newParentId, cancellationToken);
            if (updateParentResult.IsFailure)
                return updateParentResult.Error;

            transactionScope.Commit();

            return departmentId.Value;
        }
        catch (Exception ex)
        {
            transactionScope.Rollback();
            return Error.Failure(
                    "update.department.parent.failed",
                    $"Failed to update parent for department with Id {command.DepartmentId}.")
                .ToErrors();
        }
    }

    private async Task<UnitResult<ErrorList>> ChangeParentAsync(
        DepartmentId departmentId,
        DepartmentId? newParentId,
        CancellationToken cancellationToken)
    {
        if (newParentId is not null)
        {
            if (await _departmentsRepository.IsDescendantAsync(departmentId, newParentId))
            {
                return Errors.General
                    .ValueIsInvalid(
                        nameof(newParentId),
                        $"Cannot set a descendant department with Id {newParentId.Value} as parent.")
                    .ToErrors();
            }

            return await SetNewParentAsync(departmentId, newParentId, cancellationToken);
        }

        return await RemoveParentAsync(departmentId, cancellationToken);
    }

    private async Task<UnitResult<ErrorList>> SetNewParentAsync(
        DepartmentId departmentId,
        DepartmentId newParentId,
        CancellationToken cancellationToken)
    {
        var parentResult = await _departmentsRepository.GetByIdWithLockAsync(
            newParentId,
            cancellationToken);

        if (parentResult.IsFailure)
            return parentResult.Error.ToErrors();

        var departmentResult = await _departmentsRepository.GetByIdWithLockAsync(
            departmentId,
            cancellationToken);

        if (departmentResult.IsFailure)
            return departmentResult.Error.ToErrors();

        var parent = parentResult.Value;
        var department = departmentResult.Value;
        short oldDepth = department.Depth;
        var oldPath = department.Path;

        department.SetParent(newParentId, parent.Depth, parent.Path);

        var lockDescendantsResult = await _departmentsRepository.LockDescendants(
            oldPath,
            cancellationToken);

        if (lockDescendantsResult.IsFailure)
            return lockDescendantsResult.Error.ToErrors();

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error.ToErrors();

        return await _departmentsRepository.UpdateDepartmentsHierarchyAsync(
            department,
            oldDepth,
            oldPath);
    }

    private async Task<UnitResult<ErrorList>> RemoveParentAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        var departmentResult =
            await _departmentsRepository.GetByIdWithLockAsync(departmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error.ToErrors();

        var department = departmentResult.Value;

        short oldDepth = department.Depth;
        var oldPath = department.Path;
        department.RemoveParent();

        var lockDescendantsResult = await _departmentsRepository.LockDescendants(
            oldPath,
            cancellationToken);

        if (lockDescendantsResult.IsFailure)
            return lockDescendantsResult.Error.ToErrors();

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error.ToErrors();

        return await _departmentsRepository.UpdateDepartmentsHierarchyAsync(
            department,
            oldDepth,
            oldPath);
    }
}