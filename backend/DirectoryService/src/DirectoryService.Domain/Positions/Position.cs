using DirectoryService.Domain.DepartmentPositions;

namespace DirectoryService.Domain.Positions;

public sealed class Position
{
    private readonly List<DepartmentPosition> _departmentPositions = [];

    public Position(
        PositionId id,
        PositionName name,
        Description? description,
        IEnumerable<DepartmentPosition> departmentPositions)
    {
        Id = id;
        Name = name;
        Description = description;
        _departmentPositions = departmentPositions.ToList();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Position()
    {
    }

    public PositionId Id { get; } = null!;

    public PositionName Name { get; private set; } = null!;

    public Description? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime? UpdatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions.AsReadOnly();

    public void SoftDelete()
    {
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsActive = true;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
}