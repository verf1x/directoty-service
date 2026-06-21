namespace DirectoryService.Contracts.Locations;

public class LocationListRow
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string PostalCode { get; init; }

    public required string Region { get; init; }

    public required string City { get; init; }

    public string? District { get; init; }

    public required string Street { get; init; }

    public required string House { get; init; }

    public string? Building { get; init; }

    public string? Apartment { get; init; }

    public required string TimeZone { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required bool IsActive { get; init; }

    public required int DepartmentsCount { get; init; }

    public required long TotalCount { get; init; }
}