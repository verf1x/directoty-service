namespace DirectoryService.Domain.EntityIds;

public sealed class DepartmentLocationId : EntityIdBase
{
    private DepartmentLocationId(Guid value)
        : base(value)
    {
    }

    public static DepartmentLocationId CreateNew() => new(Guid.NewGuid());

    public static DepartmentLocationId CreateEmpty() => new(Guid.Empty);

    public static DepartmentLocationId Create(Guid id) => new(id);
}
