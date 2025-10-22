namespace DirectoryService.Application.Departments.Dtos;

public class DepartmentParentDto
{
    public required Guid Id { get; init; }

    public string Path { get; init; } = string.Empty;

    public short Depth { get; init; }
}