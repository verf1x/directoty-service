using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using Path = DirectoryService.Domain.Departments.Path;

namespace DirectoryService.Application.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Department, Error>> GetByIdWithLocationsAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdWithLockAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> LockDescendants(Path path, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken);

    Task<Result<DepartmentParentDto, Error>> GetDepartmentParentAsync(
        Guid parentId,
        CancellationToken cancellationToken);

    Task<bool> DepartmentWithIdentifierExistAsync(
        string identifier,
        CancellationToken cancellationToken);

    Task<bool> DepartmentActiveByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task DeleteLocationsByDepartmentIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    Task<bool> LocationsExistByIdsAsync(
        IEnumerable<LocationId> locationIds,
        CancellationToken cancellationToken);

    Task<bool> LocationsActiveByIdsAsync(
        IEnumerable<LocationId> locationIds,
        CancellationToken cancellationToken);

    Task<bool> IsDescendantAsync(
        DepartmentId currentId,
        DepartmentId newParentId);

    Task<UnitResult<ErrorList>> UpdateDepartmentsHierarchyAsync(
        Department department,
        short oldDepth,
        Path oldPath);
}