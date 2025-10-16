using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Departments;

public sealed class DepartmentId : EntityIdBase
{
    private DepartmentId(Guid value)
    : base(value)
    {
    }

    public static DepartmentId CreateNew() => new(Guid.NewGuid());

    public static DepartmentId CreateEmpty() => new(Guid.Empty);

    public static DepartmentId Create(Guid id) => new(id);
}