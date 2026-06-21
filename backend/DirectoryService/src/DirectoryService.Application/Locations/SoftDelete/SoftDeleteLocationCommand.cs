using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Locations.SoftDelete;

public record SoftDeleteLocationCommand(Guid Id) : ICommand;