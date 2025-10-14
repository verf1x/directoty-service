using CSharpFunctionalExtensions;
using DirectoryService.Domain;
using DirectoryService.Domain.Entities;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);
}