using DirectoryService.Application.Departments.Get;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts;
using DirectoryService.Domain.Shared;

namespace DirectoryService.IntegrationTests.Departments;

public class GetDepartmentsQueryValidatorTests
{
    [Fact]
    public void ToErrors_WithInvalidQuery_ShouldReturnValidationErrors()
    {
        // arrange
        var validator = new GetDepartmentsQueryValidator();
        var query = new GetDepartmentsQuery(
            new string('a', 101),
            "unsupported",
            "sideways",
            new Pagination(0, 101));

        // act
        var validationResult = validator.Validate(query);
        ErrorList errors = validationResult.ToErrors();

        // assert
        Assert.False(validationResult.IsValid);
        Assert.Equal(5, errors.Count());
        Assert.All(errors, error => Assert.Equal(ErrorType.Validation, error.Type));
    }

    [Fact]
    public void Validate_WithUppercaseSortDirection_ShouldBeValid()
    {
        // arrange
        var validator = new GetDepartmentsQueryValidator();
        var query = new GetDepartmentsQuery(null, "name", "DESC", new Pagination());

        // act
        var validationResult = validator.Validate(query);

        // assert
        Assert.True(validationResult.IsValid);
    }

    [Fact]
    public void Validate_WithOmittedSort_ShouldBeValid()
    {
        // arrange
        var validator = new GetDepartmentsQueryValidator();
        var query = new GetDepartmentsQuery(null, null!, null!, new Pagination());

        // act
        var validationResult = validator.Validate(query);

        // assert
        Assert.True(validationResult.IsValid);
    }
}
