using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Domain.DepartmentPositions;

public sealed class DepartmentPosition
{
    public DepartmentPosition(DepartmentId departmentId, PositionId positionId)
    {
        Id = DepartmentPositionId.CreateNew();
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    // EF Core
    private DepartmentPosition()
    {
    }

    public DepartmentPositionId Id { get; private set; } = null!;

    public Department Department { get; private set; } = null!;

    public DepartmentId DepartmentId { get; private set; } = null!;

    public Position Position { get; private set; } = null!;

    public PositionId PositionId { get; private set; } = null!;
}