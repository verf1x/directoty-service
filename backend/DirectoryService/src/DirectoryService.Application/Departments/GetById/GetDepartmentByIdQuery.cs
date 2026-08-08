using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Departments.GetById;

public record GetDepartmentByIdQuery(Guid Id) : IQuery;