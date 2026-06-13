namespace DirectoryService.Contracts.Locations;

public record GetLocationsResponse(List<LocationListItemDto> Locations, long TotalCount);