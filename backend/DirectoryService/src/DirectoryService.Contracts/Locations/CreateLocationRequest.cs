namespace DirectoryService.Contracts.Locations;

public record CreateLocationRequest(
    string Name,
    string PostalCode,
    string Region,
    string City,
    string? District,
    string Street,
    string House,
    string? Building,
    string? Apartment,
    string TimeZone);