using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Locations;

public sealed class LocationId : EntityIdBase
{
    private LocationId(Guid value)
    : base(value)
    {
    }

    public static LocationId CreateNew() => new(Guid.CreateVersion7());

    public static LocationId CreateEmpty() => new(Guid.Empty);

    public static LocationId Create(Guid id) => new(id);
}