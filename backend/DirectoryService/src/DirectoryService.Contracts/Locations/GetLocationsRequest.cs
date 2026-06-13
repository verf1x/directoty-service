namespace DirectoryService.Contracts.Locations;

public record GetLocationsRequest(
    Pagination Pagination,
    int? MinDepartmentsCount,
    string? Search,
    string SortBy = "name",
    string SortDirection = "asc");