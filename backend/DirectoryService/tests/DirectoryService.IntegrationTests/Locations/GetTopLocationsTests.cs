using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Application.Locations.GetTop;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;

namespace DirectoryService.IntegrationTests.Locations;

public class GetTopLocationsTests(DirectoryServiceTestsWebFactory webFactory) : BaseIntegrationTest(webFactory)
{
    private int _departmentIndex;
    private int _postalCode = 200000;

    [Fact]
    public async Task GetTopLocations_WithoutLocations_ShouldReturnEmptyList()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var query = new GetTopLocationsQuery();

        // act
        var result = await GetTopLocations(query, cancellationToken);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.TopLocations);
    }

    [Fact]
    public async Task GetTopLocations_WithMoreThanFiveLocations_ShouldReturnFiveLocationsOrderedByDepartmentsCount()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        await CreateLocation("Zero", 0, cancellationToken);
        await CreateLocation("One", 1, cancellationToken);
        await CreateLocation("Two", 2, cancellationToken);
        await CreateLocation("Three", 3, cancellationToken);
        await CreateLocation("Four", 4, cancellationToken);
        await CreateLocation("Five", 5, cancellationToken);

        var query = new GetTopLocationsQuery();

        // act
        var result = await GetTopLocations(query, cancellationToken);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value.TopLocations.Count);
        Assert.Collection(
            result.Value.TopLocations,
            location =>
            {
                Assert.Equal("Five", location.Name);
                Assert.Equal(5, location.DepartmentsCount);
            },
            location =>
            {
                Assert.Equal("Four", location.Name);
                Assert.Equal(4, location.DepartmentsCount);
            },
            location =>
            {
                Assert.Equal("Three", location.Name);
                Assert.Equal(3, location.DepartmentsCount);
            },
            location =>
            {
                Assert.Equal("Two", location.Name);
                Assert.Equal(2, location.DepartmentsCount);
            },
            location =>
            {
                Assert.Equal("One", location.Name);
                Assert.Equal(1, location.DepartmentsCount);
            });
    }

    private async Task<Guid> CreateLocation(
        string name,
        int departmentsCount,
        CancellationToken cancellationToken)
    {
        var locationResult = await ExecuteHandler<CreateLocationHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateLocationCommandFakers
                .Create()
                .Generate() with
                {
                    Name = name,
                    PostalCode = NextPostalCode(),
                    Region = $"Region {name}",
                    City = $"City {name}",
                    Street = $"Street {name}",
                    House = "1",
                };

            return sut.HandleAsync(command, cancellationToken);
        });

        Assert.True(locationResult.IsSuccess);

        for (int index = 0; index < departmentsCount; index++)
        {
            await CreateDepartment(locationResult.Value, cancellationToken);
        }

        return locationResult.Value;
    }

    private async Task CreateDepartment(Guid locationId, CancellationToken cancellationToken)
    {
        var departmentIndex = _departmentIndex++;

        var departmentResult = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = new CreateDepartmentCommand(
                $"Department {departmentIndex}",
                $"department-{NumberToWord(departmentIndex)}",
                null,
                [locationId]);

            return sut.HandleAsync(command, cancellationToken);
        });

        Assert.True(departmentResult.IsSuccess);
    }

    private async Task<Result<GetTopLocationsResponse, ErrorList>> GetTopLocations(
        GetTopLocationsQuery query,
        CancellationToken cancellationToken)
        => await ExecuteHandler<GetTopLocationsHandler, Result<GetTopLocationsResponse, ErrorList>>(sut =>
            sut.HandleAsync(query, cancellationToken));

    private string NextPostalCode() => (_postalCode++).ToString(System.Globalization.CultureInfo.InvariantCulture);

    private string NumberToWord(int number)
    {
        string[] words =
        [
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
            "ten",
            "eleven",
            "twelve",
            "thirteen",
            "fourteen",
            "fifteen",
            "sixteen",
            "seventeen",
            "eighteen",
            "nineteen",
        ];

        return words[number];
    }
}
