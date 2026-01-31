using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Locations.Get;

public record GetLocationsQuery(
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
    string SortBy,
    string SortDirection) : IQuery;