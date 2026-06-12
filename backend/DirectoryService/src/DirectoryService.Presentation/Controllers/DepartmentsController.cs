using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Departments.Get;
using DirectoryService.Application.Departments.UpdateLocations;
using DirectoryService.Application.Departments.UpdateParent;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Shared;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(400)]
    public async Task<EndpointResult<PagedResult<GetDepartmentsResponseItemDto>>> GetAsync(
        [FromQuery] GetDepartmentsRequest request,
        [FromServices] IQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentsResponseItemDto>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetDepartmentsQuery(
            request.Search,
            request.SortBy,
            request.SortDirection,
            request.Pagination);

        return await handler.HandleAsync(query, cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(409)]
    public async Task<EndpointResult<Guid>> CreateAsync(
        [FromBody] CreateDepartmentRequest request,
        [FromServices] ICommandHandler<CreateDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(
            request.Name,
            request.Identifier,
            request.ParentId,
            request.LocationIds);

        return await handler.HandleAsync(command, cancellationToken);
    }

    [HttpPatch("{departmentId}/locations")]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(409)]
    public async Task<EndpointResult<Guid>> UpdateLocationsAsync(
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentLocationsRequest request,
        [FromServices] ICommandHandler<UpdateDepartmentLocationsCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentLocationsCommand(departmentId, request.LocationIds);

        return await handler.HandleAsync(command, cancellationToken);
    }

    [HttpPut("{departmentId}/parent")]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(400)]
    public async Task<EndpointResult<Guid>> UpdateParentAsync(
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentParentRequest request,
        [FromServices] ICommandHandler<UpdateDepartmentParentCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentParentCommand(departmentId, request.ParentId);

        return await handler.HandleAsync(command, cancellationToken);
    }
}
