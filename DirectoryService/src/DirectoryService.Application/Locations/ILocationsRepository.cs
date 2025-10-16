using CSharpFunctionalExtensions;
using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByNameAsync(LocationName locationName, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByAddressAsync(Address address, CancellationToken cancellationToken);
}