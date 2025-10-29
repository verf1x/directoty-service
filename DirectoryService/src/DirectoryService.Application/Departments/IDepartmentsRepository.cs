using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Department, Error>> GetByIdWithLocationsAsync(DepartmentId departmentId,
        CancellationToken cancellationToken);

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

    Task DeleteLocationsByDepartmentIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    Task<bool> LocationActiveByIdAsync(
        Guid locationId,
        CancellationToken cancellationToken);
}