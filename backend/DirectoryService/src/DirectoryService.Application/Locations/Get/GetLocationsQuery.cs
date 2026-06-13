using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Locations.Get;

public record GetLocationsQuery(
    Pagination Pagination,
    int? MinDepartmentsCount = null,
    string? Search = "name",
    string SortBy = "name",
    string SortDirection = "ASC") : IQuery;