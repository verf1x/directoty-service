using System.Globalization;
using Bogus;
using DirectoryService.Application.Locations.Create;

namespace DirectoryService.IntegrationTests.Fakers;

public class CreateLocationCommandFakers
{
    public static Faker<CreateLocationCommand> Create()
        => new Faker<CreateLocationCommand>()
            .CustomInstantiator(f => new CreateLocationCommand(
                $"Location {f.Random.AlphaNumeric(8)}",
                f.Random.Int(100000, 999999).ToString(CultureInfo.InvariantCulture),
                $"Region {f.Random.AlphaNumeric(8)}",
                $"City {f.Random.AlphaNumeric(8)}",
                null,
                $"Street {f.Random.AlphaNumeric(8)}",
                f.Random.Int(1, 999).ToString(CultureInfo.InvariantCulture),
                null,
                null,
                "UTC"));

    public static Faker<CreateLocationCommand> CreateWithOptionalAddressDetails()
        => new Faker<CreateLocationCommand>()
            .CustomInstantiator(f => Create().Generate() with
            {
                District = $"District {f.Random.AlphaNumeric(8)}",
                Building = f.Random.Int(1, 9).ToString(CultureInfo.InvariantCulture),
                Apartment = f.Random.Int(1, 999).ToString(CultureInfo.InvariantCulture),
            });

    public static Faker<CreateLocationCommand> CreateWithMinLengthName()
        => new Faker<CreateLocationCommand>()
            .CustomInstantiator(_ => Create().Generate() with
            {
                Name = "Abc",
            });

    public static Faker<CreateLocationCommand> CreateWithMaxLengthName()
        => new Faker<CreateLocationCommand>()
            .CustomInstantiator(_ => Create().Generate() with
            {
                Name = "LocationNameLength20",
            });

    public static Faker<CreateLocationCommand> CreateWithInvalidLongName()
        => new Faker<CreateLocationCommand>()
            .CustomInstantiator(f => Create().Generate() with
            {
                Name = $"Location {f.Random.AlphaNumeric(100)}",
            });

    public static Faker<CreateLocationCommand> CreateWithInvalidPostalCode()
        => new Faker<CreateLocationCommand>()
            .CustomInstantiator(_ => Create().Generate() with
            {
                PostalCode = "12345",
            });
}
