using CSharpFunctionalExtensions;
using DirectoryService.Domain;

namespace DirectoryService.Application.Abstractions;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand
{
    Task<Result<TResponse, ErrorsList>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<ErrorsList>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}