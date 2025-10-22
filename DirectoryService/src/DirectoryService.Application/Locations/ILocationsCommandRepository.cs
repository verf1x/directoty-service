﻿using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Locations;

public interface ILocationsCommandRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);
}