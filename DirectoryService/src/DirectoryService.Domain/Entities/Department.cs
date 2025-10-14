using CSharpFunctionalExtensions;
using DirectoryService.Domain.EntityIds;
using DirectoryService.Domain.ValueObjects;
using Path = DirectoryService.Domain.ValueObjects.Path;

namespace DirectoryService.Domain.Entities;

public class Department
{
    private readonly List<DepartmentLocation> _departmentLocations = [];
    private readonly List<DepartmentPosition> _departmentPositions = [];

    private Department(
        DepartmentName name,
        Identifier identifier,
        short depth,
        Department? parent = null)
    {
        Id = DepartmentId.CreateNew();
        Name = name;
        Identifier = identifier;
        Depth = depth;
        IsActive = true;
        ParentId = parent?.Id;
        CreatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Department()
    {
    }

    public DepartmentId Id { get; private set; } = null!;

    public DepartmentName Name { get; private set; } = null!;

    public Identifier Identifier { get; private set; } = null!;

    public DepartmentId? ParentId { get; private set; }

    public Path Path { get; private set; } = null!;

    public short Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations.AsReadOnly();

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions.AsReadOnly();

    public static Result<Department, Error> Create(
        DepartmentName name,
        Identifier identifier,
        short depth,
        Department? parent = null)
    {
        if (depth < 0)
            return Errors.General.ValueIsInvalid(nameof(depth));

        var department = new Department(
            name,
            identifier,
            depth,
            parent);

        return department;
    }
}