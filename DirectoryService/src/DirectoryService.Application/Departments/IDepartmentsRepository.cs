using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken);

    Task<Result<DepartmentParentDto, Error>> GetDepartmentParentAsync(
        Guid parentId,
        CancellationToken cancellationToken);

    Task<bool> DepartmentWithIdentifierExistsAsync(
        string identifier,
        CancellationToken cancellationToken);

    Task<bool> DepartmentActiveByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}