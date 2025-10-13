using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateLocationRequest request,
        [FromServices] ICommandHandler<CreateLocationCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(
            request.Name,
            request.AddressLines,
            request.Locality,
            request.Region,
            request.PostalCode,
            request.CountryCode,
            request.TimeZone);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}