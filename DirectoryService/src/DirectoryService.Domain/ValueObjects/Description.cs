using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.PositionsManagement.ValueObjects;

public sealed class Description : ComparableValueObject
{
    public string Value { get; }

    private Description(string value) => Value = value;

    public static Result<Description, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 1000)
            return Errors.General.ValueIsInvalid(nameof(value));

        return new Description(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}