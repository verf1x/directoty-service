using CSharpFunctionalExtensions;
using DirectoryService.Domain.EntityIds;

namespace DirectoryService.Domain.Shared.Entities;

public class DepartmentLocation
{
    private DepartmentLocation(DepartmentId departmentId, LocationId locationId)
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public DepartmentId DepartmentId { get; private set; }

    public LocationId LocationId { get; private set; }

    public static Result<DepartmentLocation, Error> Create(DepartmentId departmentId, LocationId locationId)
    {
        if (departmentId.Value == Guid.Empty)
            return Errors.General.ValueIsInvalid(nameof(departmentId));

        if (locationId.Value == Guid.Empty)
            return Errors.General.ValueIsInvalid(nameof(locationId));

        return new DepartmentLocation(departmentId, locationId);
    }
}
