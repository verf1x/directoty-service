using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public sealed class TimeZone : ComparableValueObject
{
    public string Value { get; }

    private TimeZone(string value) => Value = value;

    public static Result<TimeZone, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsInvalid(nameof(value));

        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(value);
            return new TimeZone(timeZone.Id);
        }
        catch (Exception)
        {
            return Errors.General.ValueIsInvalid(nameof(value));
        }
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}