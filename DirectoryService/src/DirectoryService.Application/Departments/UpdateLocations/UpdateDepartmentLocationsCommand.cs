using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Departments.UpdateLocations;

public record UpdateDepartmentLocationsCommand(
    Guid DepartmentId,
    Guid[] LocationIds) : ICommand;