namespace DirectoryService.Contracts.Locations;

public record LocationDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public bool IsActive { get; init; }

    public string Region { get; init; } = null!;

    public string City { get; init; } = null!;

    public string District { get; init; } = null!;

    public string Street { get; init; } = null!;

    public string House { get; init; } = null!;

    public string? Building { get; init; }

    public string? Apartment { get; init; }

    public string TimeZone { get; init; } = null!;

    public DateTime CreatedAt { get; init; }
}