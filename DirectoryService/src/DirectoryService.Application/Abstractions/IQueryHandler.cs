using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery
{
    Task<Result<TResponse, Error>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default);
}

public interface IQueryHandler<TResponse>
{
    Task<Result<TResponse, Error>> HandleAsync(
        CancellationToken cancellationToken = default);
}