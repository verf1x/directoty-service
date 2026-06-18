using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Locations;

public class CreateLocationsTests(DirectoryServiceTestsWebFactory webFactory) : BaseIntegrationTest(webFactory)
{
    [Fact]
    public async Task CreateLocation_WithValidData_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var command = CreateLocationCommandFakers
            .Create()
            .Generate();

        // act
        var result = await CreateLocation(command, cancellationToken);

        // assert
        await AssertLocationCreated(command, result, cancellationToken);
    }

    [Fact]
    public async Task CreateLocation_WithOptionalAddressDetails_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var command = CreateLocationCommandFakers
            .CreateWithOptionalAddressDetails()
            .Generate();

        // act
        var result = await CreateLocation(command, cancellationToken);

        // assert
        await AssertLocationCreated(command, result, cancellationToken);
    }

    [Fact]
    public async Task CreateLocation_WithMinLengthName_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var command = CreateLocationCommandFakers
            .CreateWithMinLengthName()
            .Generate();

        // act
        var result = await CreateLocation(command, cancellationToken);

        // assert
        await AssertLocationCreated(command, result, cancellationToken);
    }

    [Fact]
    public async Task CreateLocation_WithMaxLengthName_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var command = CreateLocationCommandFakers
            .CreateWithMaxLengthName()
            .Generate();

        // act
        var result = await CreateLocation(command, cancellationToken);

        // assert
        await AssertLocationCreated(command, result, cancellationToken);
    }

    [Fact]
    public async Task CreateLocation_WithInvalidPostalCode_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var command = CreateLocationCommandFakers
            .CreateWithInvalidPostalCode()
            .Generate();

        // act
        var result = await CreateLocation(command, cancellationToken);

        // assert
        AssertLocationCreateFailed(result, ErrorType.Validation, "value.length.is.invalid");
        await AssertLocationsCount(0, cancellationToken);
    }

    [Fact]
    public async Task CreateLocation_WithDuplicateAddress_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var existingLocationCommand = CreateLocationCommandFakers
            .Create()
            .Generate();
        var existingLocationResult = await CreateLocation(existingLocationCommand, cancellationToken);
        var duplicateAddressCommand = CreateLocationCommandFakers
            .Create()
            .Generate() with
            {
                PostalCode = existingLocationCommand.PostalCode,
                Region = existingLocationCommand.Region,
                City = existingLocationCommand.City,
                District = existingLocationCommand.District,
                Street = existingLocationCommand.Street,
                House = existingLocationCommand.House,
                Building = existingLocationCommand.Building,
                Apartment = existingLocationCommand.Apartment,
            };

        // act
        var result = await CreateLocation(duplicateAddressCommand, cancellationToken);

        // assert
        await AssertLocationCreated(existingLocationCommand, existingLocationResult, cancellationToken);
        AssertLocationCreateFailed(result, ErrorType.Conflict, "location.on.address.already.exists");
        await AssertLocationsCount(1, cancellationToken);
    }

    [Fact]
    public async Task CreateLocation_WithInvalidLongName_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var command = CreateLocationCommandFakers
            .CreateWithInvalidLongName()
            .Generate();

        // act
        var result = await CreateLocation(command, cancellationToken);

        // assert
        AssertLocationCreateFailed(result, ErrorType.Validation, "value.length.is.invalid");
        await AssertLocationsCount(0, cancellationToken);
    }

    [Fact]
    public async Task CreateLocation_WithDuplicateName_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var existingLocationCommand = CreateLocationCommandFakers
            .Create()
            .Generate();
        var existingLocationResult = await CreateLocation(existingLocationCommand, cancellationToken);
        var duplicateNameCommand = CreateLocationCommandFakers
            .Create()
            .Generate() with
            {
                Name = existingLocationCommand.Name,
            };

        // act
        var result = await CreateLocation(duplicateNameCommand, cancellationToken);

        // assert
        await AssertLocationCreated(existingLocationCommand, existingLocationResult, cancellationToken);
        AssertLocationCreateFailed(result, ErrorType.Conflict, "location.with.name.already.exists");
        await AssertLocationsCount(1, cancellationToken);
    }

    private async Task<Result<Guid, ErrorList>> CreateLocation(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
        => await ExecuteHandler<CreateLocationHandler, Result<Guid, ErrorList>>(sut =>
            sut.HandleAsync(command, cancellationToken));

    private async Task AssertLocationCreated(
        CreateLocationCommand command,
        Result<Guid, ErrorList> result,
        CancellationToken cancellationToken)
    {
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        await ExecuteInDb(async dbContext =>
        {
            var location = await dbContext.Locations
                .FirstAsync(l => l.Id == LocationId.Create(result.Value), cancellationToken);

            Assert.Equal(result.Value, location.Id.Value);
            Assert.Equal(command.Name, location.Name.Value);
            Assert.Equal(command.PostalCode, location.Address.PostalCode);
            Assert.Equal(command.Region, location.Address.Region);
            Assert.Equal(command.City, location.Address.City);
            Assert.Equal(command.District, location.Address.District);
            Assert.Equal(command.Street, location.Address.Street);
            Assert.Equal(command.House, location.Address.House);
            Assert.Equal(command.Building, location.Address.Building);
            Assert.Equal(command.Apartment, location.Address.Apartment);
            Assert.Equal(command.TimeZone, location.TimeZone.Value);
        });
    }

    private void AssertLocationCreateFailed(
        Result<Guid, ErrorList> result,
        ErrorType expectedErrorType,
        string expectedErrorCode)
    {
        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, error =>
            error.Type == expectedErrorType && error.Code == expectedErrorCode);
    }

    private async Task AssertLocationsCount(
        int expectedCount,
        CancellationToken cancellationToken)
    {
        await ExecuteInDb(async dbContext =>
        {
            var locationsCount = await dbContext.Locations.CountAsync(cancellationToken);

            Assert.Equal(expectedCount, locationsCount);
        });
    }
}
