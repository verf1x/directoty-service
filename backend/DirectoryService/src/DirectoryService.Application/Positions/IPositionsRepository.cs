using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Positions;

public interface IPositionsRepository
{
    Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken);

    Task<bool> IsPositionWithNameAlreadyActive(
        string positionName, CancellationToken cancellationToken);

    Task<Result<Position, Error>> GetByAsync(
        Expression<Func<Position, bool>> predicate,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Position>> GetByDepartmentIdToDeactivateAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);
}