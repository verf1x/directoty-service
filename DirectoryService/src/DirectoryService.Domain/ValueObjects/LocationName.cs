using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.LocationsManagement.ValueObjects;

public sealed class LocationName : ComparableValueObject
{
    public string Value { get; }

    private LocationName(string value) => Value = value;

    public static Result<LocationName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is < 3 or > 120)
            return Errors.General.ValueIsInvalid(nameof(value));

        return new LocationName(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}