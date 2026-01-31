using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Application.Locations.Get;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Requests;
using DirectoryService.Domain.Shared;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(400)]
    public async Task<EndpointResult<Guid>> CreateAsync(
        [FromBody] CreateLocationRequest request,
        [FromServices] ICommandHandler<CreateLocationCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(
            request.Name,
            request.PostalCode,
            request.Region,
            request.City,
            request.District,
            request.Street,
            request.House,
            request.Building,
            request.Apartment,
            request.TimeZone);

        return await handler.HandleAsync(command, cancellationToken);
    }

    [HttpGet]
    [ProducesResponseType<Envelope<GetLocationsResponse>>(200)]
    [ProducesResponseType<Envelope>(400)]
    public async Task<EndpointResult<GetLocationsResponse>> GetAsync(
        [FromQuery] GetLocationsRequest request,
        [FromServices] IQueryHandler<GetLocationsQuery, GetLocationsResponse> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetLocationsQuery(
            request.Pagination,
            request.Search,
            request.PostalCode,
            request.Region,
            request.City,
            request.District,
            request.Street,
            request.House,
            request.Building,
            request.Apartment,
            request.TimeZone,
            request.SortBy,
            request.SortDirection);

        return await handler.HandleAsync(query, cancellationToken);
    }
}