using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Positions.Create;

public record CreatePositionCommand(
    string Name,
    string? Description,
    Guid[] DepartmentIds) : ICommand;