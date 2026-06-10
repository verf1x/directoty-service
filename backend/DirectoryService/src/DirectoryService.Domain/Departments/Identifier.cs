using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Departments;

public sealed class Identifier : ComparableValueObject
{
    public string Value { get; }

    private Identifier(string value) => Value = value;

    public static Result<Identifier, Error> Create(string value)
    {
        var latinHyphenOnlyRegex = new Regex(@"^[a-z]+(-[a-z]+)*$");

        if (string.IsNullOrWhiteSpace(value) || (value.Length is < 3 or > 150) || !latinHyphenOnlyRegex.IsMatch(value))
            return Errors.General.ValueIsInvalid(nameof(value));

        return new Identifier(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}