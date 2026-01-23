using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.DepartmentPositions;

public sealed class DepartmentPositionId : EntityIdBase
{
    private DepartmentPositionId(Guid value)
        : base(value)
    {
    }

    public static DepartmentPositionId CreateNew() => new(Guid.CreateVersion7());

    public static DepartmentPositionId CreateEmpty() => new(Guid.Empty);

    public static DepartmentPositionId Create(Guid id) => new(id);
}
