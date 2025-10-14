using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public sealed class Path : ComparableValueObject
{
    public string Value { get; }

    private Path(string value) => Value = value;

    public static Result<Path, Error> Create(string value)
    {
        var pattern = new Regex(@"^[a-z]+(?:-[a-z]+)*(?:\.[a-z]+(?:-[a-z]+)*)*$");

        if (string.IsNullOrWhiteSpace(value) || !pattern.IsMatch(value))
            return Errors.General.ValueIsInvalid(nameof(value));

        return new Path(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}