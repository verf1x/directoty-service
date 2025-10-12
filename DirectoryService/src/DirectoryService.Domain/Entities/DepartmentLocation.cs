using DirectoryService.Domain.Entities;
using DirectoryService.Domain.EntityIds;
using DirectoryService.Domain.LocationsManagement.Entities;

namespace DirectoryService.Domain.Shared.Entities;

public class DepartmentLocation
{
    public DepartmentLocation(DepartmentId departmentId, LocationId locationId)
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    // EF Core
    private DepartmentLocation()
    {
    }

    public Department Department { get; private set; } = null!;

    public DepartmentId DepartmentId { get; private set; } = null!;

    public Location Location { get; private set; } = null!;

    public LocationId LocationId { get; private set; } = null!;
}
