using Dapper;
using DirectoryService.Application.Positions;
using DirectoryService.Infrastructure.Postgres.Database;

namespace DirectoryService.Infrastructure.Postgres.Positions;

public class SqlPositionsQueryRepository : IPositionsQueryRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SqlPositionsQueryRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> IsPositionWithNameAlreadyActive(
        string positionName, CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string query = """
                             SELECT EXISTS(
                                SELECT 1 FROM positions
                                WHERE is_active = true
                                AND name = @PositionName
                             )
                             """;

        return await connection.ExecuteScalarAsync<bool>(query, new { PositionName = positionName });
    }
}