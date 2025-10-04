using System.Collections;

namespace DirectoryService.Domain.Shared;

public class ErrorsList : IEnumerable<Error>
{
    private readonly List<Error> _errors;

    public ErrorsList(IEnumerable<Error> errors)
    {
        _errors = [.. errors];
    }

    public IEnumerator<Error> GetEnumerator() => _errors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator ErrorsList(Error[] errors) => new(errors);

    public static implicit operator ErrorsList(Error error) => new([error]);
}