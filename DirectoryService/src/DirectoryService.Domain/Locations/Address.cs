using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Locations;

public sealed class Address : ComparableValueObject
{
    private Address(
        string postalCode,
        string region,
        string city,
        string? district,
        string street,
        string house,
        string? building,
        string? apartment)
    {
        PostalCode = postalCode;
        Region = region;
        City = city;
        District = district;
        Street = street;
        House = house;
        Building = building;
        Apartment = apartment;
    }

    public string PostalCode { get; }

    public string Region { get; }

    public string City { get; }

    public string? District { get; }

    public string Street { get; }

    public string House { get; }

    public string? Building { get; }

    public string? Apartment { get; }

    public static Result<Address, Error> Create(
        string postalCode,
        string region,
        string? district,
        string city,
        string street,
        string house,
        string? building,
        string? apartment)
    {
        if (string.IsNullOrWhiteSpace(postalCode) || !IsValidPostalCode(postalCode))
            return Errors.General.ValueIsInvalid(nameof(postalCode));

        if (string.IsNullOrWhiteSpace(region))
            return Errors.General.ValueIsInvalid(nameof(region));

        if (district is not null && string.IsNullOrWhiteSpace(district))
            return Errors.General.ValueIsInvalid(nameof(district));

        if (string.IsNullOrWhiteSpace(city))
            return Errors.General.ValueIsInvalid(nameof(city));

        if (string.IsNullOrWhiteSpace(street))
            return Errors.General.ValueIsInvalid(nameof(street));

        if (string.IsNullOrWhiteSpace(house))
            return Errors.General.ValueIsInvalid(nameof(house));

        if (building is not null && string.IsNullOrWhiteSpace(building))
            return Errors.General.ValueIsInvalid(nameof(building));

        if (apartment is not null && string.IsNullOrWhiteSpace(apartment))
            return Errors.General.ValueIsInvalid(nameof(apartment));

        return new Address(
            postalCode,
            region,
            city,
            district,
            street,
            house,
            building,
            apartment);
    }

    public override string ToString()
        => $"{PostalCode}, {Region}, {City}, {District}, {Street}, {House}, {Building}, {Apartment}";

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return PostalCode;
        yield return Region;
        yield return District ?? string.Empty;
        yield return City;
        yield return Street;
        yield return House;
        yield return Building ?? string.Empty;
        yield return Apartment ?? string.Empty;
    }

    private static bool IsValidPostalCode(string code)
        => code.Length == 6 && code.All(char.IsDigit);
}