namespace DirectoryService.Contracts;

public record CreateDepartmentRequest(
    string Name,
    string Identifier,
    Guid? ParentId,
    Guid[] LocationIds);