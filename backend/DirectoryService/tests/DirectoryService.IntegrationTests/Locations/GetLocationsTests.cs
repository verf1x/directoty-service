using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Application.Locations.Get;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;

namespace DirectoryService.IntegrationTests.Locations;

public class GetLocationsTests(DirectoryServiceTestsWebFactory webFactory) : BaseIntegrationTest(webFactory)
{
    private int _departmentIndex;
    private int _postalCode = 100000;

    public static IEnumerable<object[]> InvalidQueries()
    {
        yield return [new GetLocationsQuery(new Pagination(0, 20))];
        yield return [new GetLocationsQuery(new Pagination(1, 0))];
        yield return [new GetLocationsQuery(new Pagination(1, 101))];
        yield return [new GetLocationsQuery(new Pagination(), Search: string.Empty)];
        yield return [new GetLocationsQuery(new Pagination(), MinDepartmentsCount: -1)];
        yield return [new GetLocationsQuery(new Pagination(), SortBy: "invalid")];
        yield return [new GetLocationsQuery(new Pagination(), SortDirection: "invalid")];
    }

    [Fact]
    public async Task GetLocations_WithPagination_ShouldReturnPageAndTotalCount()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        await CreateLocation("Alpha", 0, cancellationToken);
        await CreateLocation("Bravo", 0, cancellationToken);
        await CreateLocation("Charlie", 0, cancellationToken);

        var query = new GetLocationsQuery(
            new Pagination(Page: 2, PageSize: 1),
            SortBy: "name",
            SortDirection: "ASC");

        // act
        var result = await GetLocations(query, cancellationToken);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(3, result.Value.TotalPages);
        Assert.True(result.Value.HasNextPage);
        Assert.True(result.Value.HasPreviousPage);
        Assert.Single(result.Value.Items);
        Assert.Equal("Bravo", result.Value.Items[0].Name);
    }

    [Fact]
    public async Task GetLocations_WithSearch_ShouldReturnMatchingLocations()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        await CreateLocation("North Hub", 0, cancellationToken);
        await CreateLocation("South Hub", 0, cancellationToken);
        await CreateLocation("Warehouse", 0, cancellationToken);

        var query = new GetLocationsQuery(
            new Pagination(),
            Search: "hub",
            SortBy: "name",
            SortDirection: "ASC");

        // act
        var result = await GetLocations(query, cancellationToken);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Collection(
            result.Value.Items,
            location => Assert.Equal("North Hub", location.Name),
            location => Assert.Equal("South Hub", location.Name));
    }

    [Fact]
    public async Task GetLocations_WithMinDepartmentsCount_ShouldReturnLocationsWithEnoughDepartments()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        await CreateLocation("Alpha", 2, cancellationToken);
        await CreateLocation("Bravo", 1, cancellationToken);
        await CreateLocation("Charlie", 0, cancellationToken);

        var query = new GetLocationsQuery(
            new Pagination(),
            MinDepartmentsCount: 2,
            SortBy: "name",
            SortDirection: "ASC");

        // act
        var result = await GetLocations(query, cancellationToken);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Items);
        Assert.Equal("Alpha", result.Value.Items[0].Name);
        Assert.Equal(2, result.Value.Items[0].DepartmentsCount);
    }

    [Fact]
    public async Task GetLocations_WithSortByDepartmentsCountDesc_ShouldReturnOrderedLocations()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        await CreateLocation("Alpha", 1, cancellationToken);
        await CreateLocation("Bravo", 3, cancellationToken);
        await CreateLocation("Charlie", 2, cancellationToken);

        var query = new GetLocationsQuery(
            new Pagination(Page: 1, PageSize: 3),
            SortBy: "departmentsCount",
            SortDirection: "DESC");

        // act
        var result = await GetLocations(query, cancellationToken);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Collection(
            result.Value.Items,
            location =>
            {
                Assert.Equal("Bravo", location.Name);
                Assert.Equal(3, location.DepartmentsCount);
            },
            location =>
            {
                Assert.Equal("Charlie", location.Name);
                Assert.Equal(2, location.DepartmentsCount);
            },
            location =>
            {
                Assert.Equal("Alpha", location.Name);
                Assert.Equal(1, location.DepartmentsCount);
            });
    }

    [Theory]
    [MemberData(nameof(InvalidQueries))]
    public async Task GetLocations_WithInvalidQuery_ShouldBeFailed(GetLocationsQuery query)
    {
        // arrange
        var cancellationToken = CancellationToken.None;

        // act
        var result = await GetLocations(query, cancellationToken);

        // assert
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
        Assert.All(result.Error, error => Assert.Equal(ErrorType.Validation, error.Type));
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

    private async Task<Result<PagedResult<LocationListItemDto>, ErrorList>> GetLocations(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
        => await ExecuteHandler<GetLocationsHandler, Result<PagedResult<LocationListItemDto>, ErrorList>>(sut =>
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
