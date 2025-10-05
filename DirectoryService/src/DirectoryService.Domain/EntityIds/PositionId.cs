namespace DirectoryService.Domain.EntityIds;

public sealed class PositionId : EntityIdBase
{
    private PositionId(Guid value)
    : base(value)
    {
    }

    public static PositionId CreateNew() => new(Guid.NewGuid());

    public static PositionId CreateEmpty() => new(Guid.Empty);

    public static PositionId Create(Guid id) => new(id);
}