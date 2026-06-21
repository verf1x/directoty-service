using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Positions.SoftDelete;

public record SoftDeletePositionCommand(Guid Id) : ICommand;