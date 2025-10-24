using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Requests;
using DirectoryService.Domain.Shared;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController : ControllerBase
{
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
}