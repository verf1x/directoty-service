using System.Data;

namespace DirectoryService.Application.Database;

public interface IDbConnectionFactory : IAsyncDisposable, IDisposable
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken);
}