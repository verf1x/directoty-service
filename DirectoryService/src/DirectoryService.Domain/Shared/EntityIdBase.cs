using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.EntityIds;

public abstract class EntityIdBase : ComparableValueObject
{
    protected EntityIdBase(Guid value) => Value = value;

    public Guid Value { get; }

    public static implicit operator Guid(EntityIdBase id)
    {
        ArgumentNullException.ThrowIfNull(id);
        return id.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}