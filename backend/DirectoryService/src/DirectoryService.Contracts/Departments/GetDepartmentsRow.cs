namespace DirectoryService.Contracts.Departments;

public class GetDepartmentsRow
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Path { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required long TotalCount { get; init; }
}
