using CSharpFunctionalExtensions;
using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Result<bool, Error>> CheckIfLocationWithNameExistsAsync(LocationName locationName, CancellationToken cancellationToken);

    Task<Result<bool, Error>> CheckIfLocationOnAddressExistsAsync(Address address, CancellationToken cancellationToken);
}