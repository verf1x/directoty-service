using DirectoryService.Domain.DepartmentLocations;

namespace DirectoryService.Domain.Locations;

public sealed class Location
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

    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations.AsReadOnly();

    public void SoftDelete()
    {
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsActive = true;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
