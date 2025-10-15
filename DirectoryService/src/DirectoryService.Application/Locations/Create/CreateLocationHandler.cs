using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Extensions;
using DirectoryService.Domain;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TimeZone = DirectoryService.Domain.ValueObjects.TimeZone;

namespace DirectoryService.Application.Locations.Create;

public sealed class CreateLocationHandler : ICommandHandler<CreateLocationCommand, Guid>
{
    private readonly IValidator<CreateLocationCommand> _validator;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(
        IValidator<CreateLocationCommand> validator,
        ILocationsRepository locationsRepository,
        ILogger<CreateLocationHandler> logger)
    {
        _validator = validator;
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var locationName = LocationName.Create(command.Name).Value;

        var address = Address.Create(
            command.AddressLines.ToList(),
            command.Locality,
            command.Region,
            command.PostalCode,
            command.CountryCode).Value;

        var timeZone = TimeZone.Create(command.TimeZone).Value;

        var location = new Location(locationName, address, timeZone);

        var addResult = await _locationsRepository.AddAsync(location, cancellationToken);
        if (addResult.IsFailure)
            return addResult.Error.ToErrors();

        _logger.LogInformation("Location {LocationId} created", location.Id);

        return location.Id.Value;
    }
}