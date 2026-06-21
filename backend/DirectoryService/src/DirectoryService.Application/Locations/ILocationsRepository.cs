using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);

    Task<bool> LocationWithNameExistsAsync(
        LocationName locationName,
        CancellationToken cancellationToken);

    Task<bool> LocationOnAddressExistsAsync(Address address, CancellationToken cancellationToken);

    Task<bool> LocationExistsByIdAsync(
        Guid locationId,
        CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByAsync(
        Expression<Func<Location, bool>> predicate,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Location>> GetByDepartmentIdToDeactivateAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);
}