using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public class Path : ComparableValueObject
{
    public string Value { get; }

    private Path(string value) => Value = value;

    public static Result<Path, Error> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return 
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}