namespace DirectoryService.Contracts.Locations;

public record GetLocationsRequest(
    Pagination Pagination,
    int? MinDepartmentsCount,
    string? Search,
    string SortBy = "name, created_at",
    string SortDirection = "asc");