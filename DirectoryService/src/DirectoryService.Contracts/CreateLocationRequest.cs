namespace DirectoryService.Contracts;

public record CreateLocationRequest(
    string Name,
    IEnumerable<string> AddressLines,
    string Locality,
    string? Region,
    string? PostalCode,
    string CountryCode,
    string TimeZone);