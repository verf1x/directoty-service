namespace DirectoryService.Contracts.Departments;

public class DepartmentParentDto
{
    public required Guid Id { get; init; }

    public string Path { get; init; } = string.Empty;

    public short Depth { get; init; }
}