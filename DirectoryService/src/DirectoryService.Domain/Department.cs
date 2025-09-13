using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain;

public class Department
{
    public Guid Id { get; private set; }

    public Name Name { get; private set; } = null!;

    public string Identifier { get; private set; } = null!;

    public Guid? ParentId { get; private set; }

    public Path Path { get; private set; } = null!;

    public short Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }
}