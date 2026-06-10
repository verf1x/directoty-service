using Bogus;
using DirectoryService.Domain.Locations;
using TimeZone = DirectoryService.Domain.Locations.TimeZone;

namespace DirectoryService.IntegrationTests.Fakers;

public static class LocationFaker
{
    public static Faker<Location> Default => new Faker<Location>("ru")
        .CustomInstantiator(f => new Location(
            LocationName.Create(f.Address.City()).Value,
            Address.Create(
                f.Address.ZipCode(),
                f.Address.State(),
                f.Address.City(),
                f.Address.Country(),
                f.Address.Country(),
                f.Address.BuildingNumber(),
                null,
                f.Address.BuildingNumber()).Value,
            TimeZone.Create(f.Date.TimeZoneString()).Value));
}