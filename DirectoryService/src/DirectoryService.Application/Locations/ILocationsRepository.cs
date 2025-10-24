using CSharpFunctionalExtensions;
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
}