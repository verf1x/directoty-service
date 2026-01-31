namespace DirectoryService.Contracts.Locations;

public record GetLocationsResponse(List<LocationDto> Locations, long TotalCount);