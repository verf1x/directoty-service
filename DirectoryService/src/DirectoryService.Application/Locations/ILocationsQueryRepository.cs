using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Locations;

public interface ILocationsQueryRepository
{
    Task<bool> LocationWithNameExistsAsync(
        LocationName locationName,
        CancellationToken cancellationToken);

    Task<bool> LocationOnAddressExistsAsync(Address address, CancellationToken cancellationToken);

    Task<bool> LocationExistsByIdAsync(
        Guid locationId,
        CancellationToken cancellationToken);
}