using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public sealed class DepartmentName : ComparableValueObject
{
    public string Value { get; }

    private DepartmentName(string value) => Value = value;

    public static Result<DepartmentName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is < 3 or > 150)
            return Errors.General.ValueIsInvalid(nameof(value));

        return new DepartmentName(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}