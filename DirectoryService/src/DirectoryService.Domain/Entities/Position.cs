using DirectoryService.Domain.EntityIds;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

public sealed class Position
{
    private readonly List<DepartmentPosition> _departmentPositions = [];

    public Position(PositionName name, Description description)
    {
        Id = PositionId.CreateNew();
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Position()
    {
    }

    public PositionId Id { get; } = null!;

    public PositionName Name { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions.AsReadOnly();
}
