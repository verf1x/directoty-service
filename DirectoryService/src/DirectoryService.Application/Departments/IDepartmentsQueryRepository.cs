using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Dtos;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Departments;

public interface IDepartmentsQueryRepository
{
    Task<Result<DepartmentParentDto, Error>> GetDepartmentParentAsync(
        Guid parentId,
        CancellationToken cancellationToken);

    Task<bool> IsChildrenWithIdentifierExists(
        Guid parentId,
        string identifier,
        CancellationToken cancellationToken);
}