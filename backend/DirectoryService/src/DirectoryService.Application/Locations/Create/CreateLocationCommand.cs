using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Locations.Create;

public record CreateLocationCommand(
    string Name,
    string PostalCode,
    string Region,
    string City,
    string? District,
    string Street,
    string House,
    string? Building,
    string? Apartment,
    string TimeZone) : ICommand;