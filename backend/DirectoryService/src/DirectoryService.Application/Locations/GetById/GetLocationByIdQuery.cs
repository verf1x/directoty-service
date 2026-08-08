using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Locations.GetById;

public record GetLocationByIdQuery(Guid Id) : IQuery;