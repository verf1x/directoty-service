namespace DirectoryService.Domain.EntityIds;

public sealed class DepartmentPositionId : EntityIdBase
{
    private DepartmentPositionId(Guid value)
        : base(value)
    {
    }

    public static DepartmentPositionId CreateNew() => new(Guid.NewGuid());

    public static DepartmentPositionId CreateEmpty() => new(Guid.Empty);

    public static DepartmentPositionId Create(Guid id) => new(id);
}
