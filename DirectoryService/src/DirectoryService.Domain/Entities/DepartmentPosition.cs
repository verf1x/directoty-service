using DirectoryService.Domain.Entities;
using DirectoryService.Domain.EntityIds;
using DirectoryService.Domain.PositionsManagement.Entities;

namespace DirectoryService.Domain.Shared.Entities;

public class DepartmentPosition
{
    public DepartmentPosition(DepartmentId departmentId, PositionId positionId)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    // EF Core
    private DepartmentPosition()
    {
    }

    public Department Department { get; private set; } = null!;

    public DepartmentId DepartmentId { get; private set; } = null!;

    public Position Position { get; private set; } = null!;

    public PositionId PositionId { get; private set; } = null!;
}