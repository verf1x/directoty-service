using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Path = DirectoryService.Domain.Departments.Path;

namespace DirectoryService.Application.Departments.Create;

public sealed class CreateDepartmentHandler : ICommandHandler<CreateDepartmentCommand, Guid>
{
    private readonly IValidator<CreateDepartmentCommand> _validator;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILogger<CreateDepartmentHandler> _logger;

    public CreateDepartmentHandler(
        IValidator<CreateDepartmentCommand> validator,
        IDepartmentsRepository departmentsRepository,
        ILogger<CreateDepartmentHandler> logger)
    {
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var locationIds = command.LocationIds.Select(LocationId.Create).ToList();

        bool locationsExist = await _departmentsRepository
            .LocationsExistByIdsAsync(locationIds, cancellationToken);

        if (!locationsExist)
            return Errors.General.NotFound().ToErrors();

        Department department;

        if (command.ParentId.HasValue)
        {
            var childDepartmentResult = await CreateChildDepartment(command, cancellationToken);
            if (childDepartmentResult.IsFailure)
                return childDepartmentResult.Error;

            department = childDepartmentResult.Value;
        }
        else
        {
            var rootDepartmentResult = await CreateRootDepartment(command, cancellationToken);
            if (rootDepartmentResult.IsFailure)
                return rootDepartmentResult.Error.ToErrors();

            department = rootDepartmentResult.Value;
        }

        var result = await _departmentsRepository.AddAsync(department, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToErrors();

        _logger.LogInformation("Created department {Name}", department.Name);

        return result.Value;
    }

    private async Task<Result<Department, Error>> CreateRootDepartment(
        CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        bool isUniqueIdentifier =
            !await _departmentsRepository.DepartmentWithIdentifierExistAsync(
                command.Identifier,
                cancellationToken);

        if (!isUniqueIdentifier)
            return Errors.General.Conflict();

        var identifier = Identifier.Create(command.Identifier).Value;

        var departmentName = DepartmentName.Create(command.Name).Value;

        var departmentId = DepartmentId.CreateNew();

        var departmentLocations = CreateDepartmentLocations(command, departmentId);

        return Department.CreateRoot(
            departmentId,
            departmentName,
            identifier,
            departmentLocations).Value;
    }

    private async Task<Result<Department, ErrorList>> CreateChildDepartment(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var parentDepartmentDtoResult =
            await _departmentsRepository.GetDepartmentParentAsync(command.ParentId!.Value, cancellationToken);

        if (parentDepartmentDtoResult.IsFailure)
            return parentDepartmentDtoResult.Error.ToErrors();

        bool isUniqueIdentifier =
            !await _departmentsRepository.DepartmentWithIdentifierExistAsync(
                command.Identifier,
                cancellationToken);

        if (!isUniqueIdentifier)
            return Errors.General.Conflict().ToErrors();

        var identifier = Identifier.Create(command.Identifier).Value;

        var departmentName = DepartmentName.Create(command.Name).Value;

        var departmentId = DepartmentId.CreateNew();

        var departmentLocations = CreateDepartmentLocations(command, departmentId);

        var path = BuildChildPath(parentDepartmentDtoResult.Value.Path, identifier);

        return Department.CreateChild(
            departmentId,
            departmentName,
            identifier,
            DepartmentId.Create(parentDepartmentDtoResult.Value.Id),
            path,
            (short)(parentDepartmentDtoResult.Value.Depth + 1),
            departmentLocations).Value;
    }

    private List<DepartmentLocation> CreateDepartmentLocations(
        CreateDepartmentCommand command,
        DepartmentId departmentId)
    {
        var departmentLocations = command.LocationIds
            .Select(l => new DepartmentLocation(departmentId, LocationId.Create(l)))
            .ToList();

        return departmentLocations;
    }

    private Path BuildChildPath(string parentPath, Identifier identifier)
    {
        return Path.Create($"{parentPath}.{identifier.Value}").Value;
    }
}