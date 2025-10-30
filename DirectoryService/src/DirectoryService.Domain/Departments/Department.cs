using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    private readonly List<DepartmentLocation> _departmentLocations = [];
    private readonly List<DepartmentPosition> _departmentPositions = [];

    private Department(
        DepartmentId departmentId,
        DepartmentName name,
        Identifier identifier,
        List<DepartmentLocation> departmentLocations,
        short depth = 0,
        DepartmentId? parentId = null,
        Path? path = null)
    {
        Id = departmentId;
        Name = name;
        Identifier = identifier;
        _departmentLocations = departmentLocations;
        Depth = depth;
        ParentId = parentId;
        Path = path ?? Path.Create(identifier.Value).Value;
        IsActive = true;
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

    public static Result<Department, Error> CreateRoot(
        DepartmentId departmentId,
        DepartmentName name,
        Identifier identifier,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        return new Department(
            departmentId,
            name,
            identifier,
            departmentLocations.ToList());
    }

    public static Result<Department, Error> CreateChild(
        DepartmentId departmentId,
        DepartmentName name,
        Identifier identifier,
        DepartmentId parentId,
        Path path,
        short depth,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        if (depth <= 1)
            return Errors.General.ValueIsInvalid(nameof(depth));

        return new Department(
            departmentId,
            name,
            identifier,
            departmentLocations.ToList(),
            depth,
            parentId,
            path);
    }

    public UnitResult<Error> UpdateLocations(IEnumerable<DepartmentLocation> departmentLocations)
    {
        _departmentLocations.Clear();
        _departmentLocations.AddRange(departmentLocations);

        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }
}