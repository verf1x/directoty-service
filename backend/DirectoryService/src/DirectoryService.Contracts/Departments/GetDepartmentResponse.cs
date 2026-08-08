namespace DirectoryService.Contracts.Departments;

public record GetDepartmentResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Identifier { get; init; }

    public required string Path { get; init; }

    public required DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}