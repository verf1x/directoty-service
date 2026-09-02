using System.Data;
using DirectoryService.Application.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres.Database;

public class NpgSqlReadConnectionFactory : IReadDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgSqlReadConnectionFactory(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        string connectionString = configuration.GetConnectionString("DirectoryServiceReadDb")
                                  ?? throw new InvalidOperationException(
                                      "Connection string 'DirectoryServiceReadDb' is not configured.");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder
            .UseLoggerFactory(loggerFactory);

        _dataSource = dataSourceBuilder.Build();
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        return await _dataSource.OpenConnectionAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dataSource.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _dataSource.DisposeAsync();
    }
}