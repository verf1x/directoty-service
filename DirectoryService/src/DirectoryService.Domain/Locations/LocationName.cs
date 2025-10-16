using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Locations;

public sealed class LocationName : ComparableValueObject
{
    public string Value { get; }

    private LocationName(string value) => Value = value;

    public static Result<LocationName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.Validation.CannotBeNullOrEmpty(nameof(value));

        if (value.Length is < 3 or > 20)
            return Errors.Validation.InvalidLength(nameof(value), 3, 20);

        return new LocationName(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}