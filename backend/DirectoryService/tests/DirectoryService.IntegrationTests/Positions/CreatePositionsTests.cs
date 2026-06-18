using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.Create;
using DirectoryService.Application.Locations.Create;
using DirectoryService.Application.Positions.Create;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Shared;
using DirectoryService.IntegrationTests.Fakers;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Positions;

public class CreatePositionsTests(DirectoryServiceTestsWebFactory webFactory) : BaseIntegrationTest(webFactory)
{
    [Fact]
    public async Task CreatePosition_WithValidData_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var departmentId = await CreateDepartment(cancellationToken);
        var command = CreatePositionCommandFakers
            .Create([departmentId])
            .Generate();

        // act
        var result = await CreatePosition(command, cancellationToken);

        // assert
        await AssertPositionCreated(command, result, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithoutDescription_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var departmentId = await CreateDepartment(cancellationToken);
        var command = CreatePositionCommandFakers
            .CreateWithoutDescription([departmentId])
            .Generate();

        // act
        var result = await CreatePosition(command, cancellationToken);

        // assert
        await AssertPositionCreated(command, result, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithTwoDepartments_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var departmentId1 = await CreateDepartment(cancellationToken);
        var departmentId2 = await CreateDepartment(cancellationToken);
        var command = CreatePositionCommandFakers
            .Create([departmentId1, departmentId2])
            .Generate();

        // act
        var result = await CreatePosition(command, cancellationToken);

        // assert
        await AssertPositionCreated(command, result, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithBoundaryNameLengths_ShouldBeSucceed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var departmentId1 = await CreateDepartment(cancellationToken);
        var departmentId2 = await CreateDepartment(cancellationToken);
        var minLengthNameCommand = CreatePositionCommandFakers
            .CreateWithMinLengthName([departmentId1])
            .Generate();
        var maxLengthNameCommand = CreatePositionCommandFakers
            .CreateWithMaxLengthName([departmentId2])
            .Generate();

        // act
        var minLengthNameResult = await CreatePosition(minLengthNameCommand, cancellationToken);
        var maxLengthNameResult = await CreatePosition(maxLengthNameCommand, cancellationToken);

        // assert
        await AssertPositionCreated(minLengthNameCommand, minLengthNameResult, cancellationToken);
        await AssertPositionCreated(maxLengthNameCommand, maxLengthNameResult, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithInvalidLongName_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var departmentId = await CreateDepartment(cancellationToken);
        var command = CreatePositionCommandFakers
            .CreateWithInvalidLongName([departmentId])
            .Generate();

        // act
        var result = await CreatePosition(command, cancellationToken);

        // assert
        AssertPositionCreateFailed(result, ErrorType.Validation, "value.is.invalid");
        await AssertPositionsCount(0, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithInvalidLongDescription_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var departmentId = await CreateDepartment(cancellationToken);
        var command = CreatePositionCommandFakers
            .CreateWithInvalidLongDescription([departmentId])
            .Generate();

        // act
        var result = await CreatePosition(command, cancellationToken);

        // assert
        AssertPositionCreateFailed(result, ErrorType.Validation, "value.is.invalid");
        await AssertPositionsCount(0, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithInvalidEmptyDescription_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var departmentId = await CreateDepartment(cancellationToken);
        var command = CreatePositionCommandFakers
            .CreateWithInvalidEmptyDescription([departmentId])
            .Generate();

        // act
        var result = await CreatePosition(command, cancellationToken);

        // assert
        AssertPositionCreateFailed(result, ErrorType.Validation, "value.is.invalid");
        await AssertPositionsCount(0, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithEmptyDepartmentIds_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var command = CreatePositionCommandFakers
            .Create([])
            .Generate();

        // act
        var result = await CreatePosition(command, cancellationToken);

        // assert
        AssertPositionCreateFailed(result, ErrorType.Validation, "value.length.is.invalid");
        await AssertPositionsCount(0, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithDuplicateDepartmentIds_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var departmentId = await CreateDepartment(cancellationToken);
        var command = CreatePositionCommandFakers
            .Create([departmentId, departmentId])
            .Generate();

        // act
        var result = await CreatePosition(command, cancellationToken);

        // assert
        AssertPositionCreateFailed(result, ErrorType.Validation, "value.already.exists");
        await AssertPositionsCount(0, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithNonExistentDepartmentId_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var command = CreatePositionCommandFakers
            .Create([Guid.CreateVersion7()])
            .Generate();

        // act
        var result = await CreatePosition(command, cancellationToken);

        // assert
        AssertPositionCreateFailed(result, ErrorType.Validation, "value.is.invalid");
        await AssertPositionsCount(0, cancellationToken);
    }

    [Fact]
    public async Task CreatePosition_WithDuplicateName_ShouldBeFailed()
    {
        // arrange
        var cancellationToken = CancellationToken.None;
        var departmentId1 = await CreateDepartment(cancellationToken);
        var departmentId2 = await CreateDepartment(cancellationToken);
        var existingPositionCommand = CreatePositionCommandFakers
            .Create([departmentId1])
            .Generate();
        var existingPositionResult = await CreatePosition(existingPositionCommand, cancellationToken);
        var duplicateNameCommand = CreatePositionCommandFakers
            .Create([departmentId2])
            .Generate() with
            {
                Name = existingPositionCommand.Name,
            };

        // act
        var result = await CreatePosition(duplicateNameCommand, cancellationToken);

        // assert
        await AssertPositionCreated(existingPositionCommand, existingPositionResult, cancellationToken);
        AssertPositionCreateFailed(result, ErrorType.Conflict, "value.already.exists");
        await AssertPositionsCount(1, cancellationToken);
    }

    private async Task<Guid> CreateDepartment(CancellationToken cancellationToken)
    {
        var locationResult = await ExecuteHandler<CreateLocationHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateLocationCommandFakers
                .Create()
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        Assert.True(locationResult.IsSuccess);

        var departmentResult = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, ErrorList>>(sut =>
        {
            var command = CreateDepartmentCommandFaker
                .CreateRoot([locationResult.Value])
                .Generate();

            return sut.HandleAsync(command, cancellationToken);
        });

        Assert.True(departmentResult.IsSuccess);

        return departmentResult.Value;
    }

    private async Task<Result<Guid, ErrorList>> CreatePosition(
        CreatePositionCommand command,
        CancellationToken cancellationToken)
        => await ExecuteHandler<CreatePositionHandler, Result<Guid, ErrorList>>(sut =>
            sut.HandleAsync(command, cancellationToken));

    private async Task AssertPositionCreated(
        CreatePositionCommand command,
        Result<Guid, ErrorList> result,
        CancellationToken cancellationToken)
    {
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        await ExecuteInDb(async dbContext =>
        {
            var position = await dbContext.Positions
                .Include(p => p.DepartmentPositions)
                .FirstAsync(p => p.Id == PositionId.Create(result.Value), cancellationToken);

            Assert.Equal(result.Value, position.Id.Value);
            Assert.Equal(command.Name, position.Name.Value);
            Assert.Equal(command.Description, position.Description?.Value);
            Assert.Equal(command.DepartmentIds.Length, position.DepartmentPositions.Count);

            foreach (var departmentId in command.DepartmentIds)
            {
                Assert.Contains(
                    position.DepartmentPositions,
                    departmentPosition => departmentPosition.DepartmentId.Value == departmentId);
            }
        });
    }

    private void AssertPositionCreateFailed(
        Result<Guid, ErrorList> result,
        ErrorType expectedErrorType,
        string expectedErrorCode)
    {
        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, error =>
            error.Type == expectedErrorType && error.Code == expectedErrorCode);
    }

    private async Task AssertPositionsCount(
        int expectedCount,
        CancellationToken cancellationToken)
    {
        await ExecuteInDb(async dbContext =>
        {
            var positionsCount = await dbContext.Positions.CountAsync(cancellationToken);
            var departmentPositionsCount = await dbContext.Set<Domain.DepartmentPositions.DepartmentPosition>()
                .CountAsync(cancellationToken);

            Assert.Equal(expectedCount, positionsCount);
            Assert.Equal(expectedCount, departmentPositionsCount);
        });
    }
}
