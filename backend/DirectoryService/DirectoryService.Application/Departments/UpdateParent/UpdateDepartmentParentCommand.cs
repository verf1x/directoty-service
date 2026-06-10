using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Departments.UpdateParent;

public sealed record UpdateDepartmentParentCommand(Guid DepartmentId, Guid? ParentId) : ICommand;