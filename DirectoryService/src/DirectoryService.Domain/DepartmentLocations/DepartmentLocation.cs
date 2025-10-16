using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Domain.DepartmentLocations;

public sealed class DepartmentLocation
{
    public DepartmentLocation(DepartmentId departmentId, LocationId locationId)
    {
        Id = DepartmentLocationId.CreateNew();
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    // EF Core
    private DepartmentLocation()
    {
    }

    public DepartmentLocationId Id { get; private set; } = null!;

    public Department Department { get; private set; } = null!;

    public DepartmentId DepartmentId { get; private set; } = null!;

    public Location Location { get; private set; } = null!;

    public LocationId LocationId { get; private set; } = null!;
}
