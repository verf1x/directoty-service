namespace DirectoryService.Contracts.Locations;

public record GetLocationsRequest(
    PaginationRequest Pagination,
    string? Search,
    string? PostalCode,
    string? Region,
    string? City,
    string? District,
    string? Street,
    string? House,
    string? Building,
    string? Apartment,
    string? TimeZone,
    string SortBy = "name, created_at",
    string SortDirection = "asc");