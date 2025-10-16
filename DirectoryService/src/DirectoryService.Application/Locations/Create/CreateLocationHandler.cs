using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TimeZone = DirectoryService.Domain.Locations.TimeZone;

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

        var existingLocationByNameResult = await _locationsRepository
            .GetByNameAsync(locationName, cancellationToken);

        if (existingLocationByNameResult.IsSuccess)
        {
            return Error.Conflict(
                "location.with.name.already.exists",
                $"Location with name {command.Name} already exists").ToErrors();
        }

        var address = Address.Create(
            command.PostalCode,
            command.Region,
            command.City,
            command.District,
            command.Street,
            command.House,
            command.Building,
            command.Apartment).Value;

        var existingLocationByAddressResult = await _locationsRepository
            .GetByAddressAsync(address, cancellationToken);

        if (existingLocationByAddressResult.IsSuccess)
        {
            return Error.Conflict(
                "location.with.address.already.exists",
                $"Location on address {address} already exists").ToErrors();
        }

        var timeZone = TimeZone.Create(command.TimeZone).Value;

        var location = new Location(locationName, address, timeZone);

        var addResult = await _locationsRepository.AddAsync(location, cancellationToken);
        if (addResult.IsFailure)
            return addResult.Error.ToErrors();

        _logger.LogInformation("Location {LocationId} created", location.Id);

        return location.Id.Value;
    }
}