using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Positions.Create;

public class CreatePositionHandler : ICommandHandler<CreatePositionCommand, Guid>
{
    private readonly IValidator<CreatePositionCommand> _validator;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly IPositionsRepository _positionsRepository;
    private readonly ILogger<CreatePositionHandler> _logger;

    public CreatePositionHandler(
        IValidator<CreatePositionCommand> validator,
        IDepartmentsRepository departmentsRepository,
        IPositionsRepository positionsRepository,
        ILogger<CreatePositionHandler> logger)
    {
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _positionsRepository = positionsRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreatePositionCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        bool positionWithNameAlreadyActive =
            await _positionsRepository.IsPositionWithNameAlreadyActive(command.Name, cancellationToken);

        if (positionWithNameAlreadyActive)
            return Errors.General.Conflict().ToErrors();

        var name = PositionName.Create(command.Name).Value;

        bool departmentIdsValid = (await Task.WhenAll(
                command.DepartmentIds.Select(d =>
                    _departmentsRepository.DepartmentActiveByIdAsync(d, cancellationToken))))
            .All(exists => exists);

        if (!departmentIdsValid)
            return Errors.General.ValueIsInvalid(nameof(command.DepartmentIds)).ToErrors();

        var description = command.Description is not null
            ? Description.Create(command.Description).Value
            : null;

        var positionId = PositionId.CreateNew();

        var departmentPositions = command.DepartmentIds
            .Select(dp => new DepartmentPosition(DepartmentId.Create(dp), positionId));

        var position = new Position(positionId, name, description, departmentPositions);

        var result = await _positionsRepository.AddAsync(position, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToErrors();

        _logger.LogInformation("Position created with ID {PositionId}", position.Id);

        return result.Value;
    }
}