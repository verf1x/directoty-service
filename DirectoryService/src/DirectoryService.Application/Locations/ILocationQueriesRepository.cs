using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Locations;

public interface ILocationQueriesRepository
{
    Task<bool> CheckIfLocationWithNameExistsAsync(
        LocationName locationName,
        CancellationToken cancellationToken);

    Task<bool> CheckIfLocationOnAddressExistsAsync(Address address, CancellationToken cancellationToken);
}