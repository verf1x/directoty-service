using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.PositionsManagement.ValueObjects;

public sealed class PositionName : ComparableValueObject
{
    public string Value { get; }

    private PositionName(string value) => Value = value;

    public static Result<PositionName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is < 3 or > 100)
            return Errors.General.ValueIsInvalid(nameof(value));

        return new PositionName(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}