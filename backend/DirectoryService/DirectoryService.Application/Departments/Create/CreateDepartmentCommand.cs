using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Departments.Create;

public record CreateDepartmentCommand(
    string Name,
    string Identifier,
    Guid? ParentId,
    Guid[] LocationIds) : ICommand;