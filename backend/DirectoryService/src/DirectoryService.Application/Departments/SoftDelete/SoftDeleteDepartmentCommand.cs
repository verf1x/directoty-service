using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Departments.SoftDelete;

public record SoftDeleteDepartmentCommand(Guid Id) : ICommand;