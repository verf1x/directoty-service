using DirectoryService.Domain.EntityIds;
using DirectoryService.Domain.ValueObjects;
using TimeZone = DirectoryService.Domain.ValueObjects.TimeZone;

namespace DirectoryService.Domain.Entities;

public class Location
{
    private readonly List<DepartmentLocation> _departmentLocations = [];

    public Location(LocationName name, Address address, TimeZone timeZone)
    {
        Id = LocationId.CreateNew();
        Name = name;
        Address = address;
        TimeZone = timeZone;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Location()
    {
    }

    public LocationId Id { get; } = null!;

    public LocationName Name { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public TimeZone TimeZone { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations.AsReadOnly();
}
