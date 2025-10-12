using DirectoryService.Domain.Entities;
using DirectoryService.Domain.EntityIds;
using DirectoryService.Domain.PositionsManagement.Entities;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Shared.Entities;

public class DepartmentPosition
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