using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Database;

public class EfCoreTransaction : ITransactionScope
{
    private readonly IDbContextTransaction _transaction;
    private readonly ILogger<EfCoreTransaction> _logger;
    private bool _completed;

    public EfCoreTransaction(IDbContextTransaction transaction, ILogger<EfCoreTransaction> logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _transaction.CommitAsync(cancellationToken);
            _completed = true;

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit transaction.");
            return Error.Failure("transaction.commit.failed", "Failed to commit transaction.");
        }
    }

    public async Task<UnitResult<Error>> RollbackAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _transaction.RollbackAsync(cancellationToken);
            _completed = true;

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rollback transaction.");
            return Error.Failure("transaction.rollback.failed", "Failed to rollback transaction.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_completed)
        {
            try
            {
                await _transaction.RollbackAsync();
                _completed = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to rollback database transaction during dispose");
            }
        }

        await _transaction.DisposeAsync();
    }
}