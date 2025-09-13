using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public class Name : ComparableValueObject
{
    public string Value { get; }

    private Name(string value) => Value = value;

    public static Result<Name, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) && value.Length is >= 3 and <= 150)
            return Errors.General.ValueIsInvalid(nameof(value));

        return new Name(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}