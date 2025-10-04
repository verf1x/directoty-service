using CSharpFunctionalExtensions;
using DirectoryService.Domain.EntityIds;

namespace DirectoryService.Domain.Shared.Entities;

public class DepartmentPosition
{
    private DepartmentPosition(DepartmentId departmentId, PositionId positionId)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public DepartmentId DepartmentId { get; private set; }

    public PositionId PositionId { get; private set; }

    public static Result<DepartmentPosition, Error> Create(DepartmentId departmentId, PositionId positionId)
    {
        if (departmentId.Value == Guid.Empty)
            return Errors.General.ValueIsInvalid(nameof(departmentId));

        if (positionId.Value == Guid.Empty)
            return Errors.General.ValueIsInvalid(nameof(positionId));

        return new DepartmentPosition(departmentId, positionId);
    }
}