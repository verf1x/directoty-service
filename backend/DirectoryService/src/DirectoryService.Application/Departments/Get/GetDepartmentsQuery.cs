using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Departments.Get;

public record GetDepartmentsQuery(
    string? Search,
    string SortBy,
    string SortDirection,
    Pagination Pagination) : IQuery;