using DirectoryService.Domain.EntityIds;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

public class DepartmentLocation
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
