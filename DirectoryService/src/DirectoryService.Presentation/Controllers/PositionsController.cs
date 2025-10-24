using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Positions.Create;
using DirectoryService.Contracts.Requests;
using DirectoryService.Domain.Shared;
using DirectoryService.Presentation.Response;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Envelope<Guid>>(200)]
    [ProducesResponseType<Envelope>(400)]
    [ProducesResponseType<Envelope>(409)]
    public async Task<EndpointResult<Guid>> CreateAsync(
        [FromBody] CreatePositionRequest request,
        [FromServices] ICommandHandler<CreatePositionCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(
            request.Name,
            request.Description,
            request.DepartmentIds);

        return await handler.HandleAsync(command, cancellationToken);
    }
}