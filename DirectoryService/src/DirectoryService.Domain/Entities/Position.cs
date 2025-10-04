using DirectoryService.Domain.EntityIds;
using DirectoryService.Domain.PositionsManagement.ValueObjects;
using DirectoryService.Domain.Shared.Entities;

namespace DirectoryService.Domain.PositionsManagement.Entities;

public class Position
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

    public PositionId Id { get; }

    public PositionName Name { get; private set; }

    public Description Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions.AsReadOnly();
}
