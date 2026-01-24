using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Positions;

public sealed class PositionId : EntityIdBase
{
    private PositionId(Guid value)
    : base(value)
    {
    }

    public static PositionId CreateNew() => new(Guid.CreateVersion7());

    public static PositionId CreateEmpty() => new(Guid.Empty);

    public static PositionId Create(Guid id) => new(id);
}