namespace DirectoryService.Application.Positions;

public interface IPositionsQueryRepository
{
    Task<bool> IsPositionWithNameAlreadyActive(
        string positionName, CancellationToken cancellationToken);
}