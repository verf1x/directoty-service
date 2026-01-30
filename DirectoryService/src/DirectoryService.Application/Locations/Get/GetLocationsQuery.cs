using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Locations.Get;

public record GetLocationsQuery() : IQuery;

public record PaginationRequest(int PageNumber = 1, int PageSize = 20);

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
    }
}

public class GetLocationsHandler : IQueryHandler<GetLocationsQuery, GetLocationsResponse>
{
    public async Task<Result<GetLocationsResponse, Error>> HandleAsync(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
    }
}