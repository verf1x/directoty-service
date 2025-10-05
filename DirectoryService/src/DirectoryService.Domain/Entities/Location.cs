using DirectoryService.Domain.EntityIds;
using DirectoryService.Domain.LocationsManagement.ValueObjects;
using DirectoryService.Domain.Shared.Entities;
using TimeZone = DirectoryService.Domain.LocationsManagement.ValueObjects.TimeZone;

namespace DirectoryService.Domain.LocationsManagement.Entities;

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

    public LocationId Id { get; }

    public LocationName Name { get; private set; }

    public Address Address { get; private set; }

    public TimeZone TimeZone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations.AsReadOnly();
}
