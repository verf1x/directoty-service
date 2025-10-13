using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Locations.Create;

public record CreateLocationCommand(
    string Name,
    IEnumerable<string> AddressLines,
    string Locality,
    string? Region,
    string? PostalCode,
    string CountryCode,
    string TimeZone) : ICommand;