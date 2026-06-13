using DirectoryService.Application.Validation;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Locations.Get;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(x => x.Pagination.Page)
            .GreaterThan(0)
            .WithError(Errors.Validation.InvalidExpectedLength(nameof(GetLocationsQuery.Pagination.Page)));

        RuleFor(x => x.Pagination.PageSize)
            .GreaterThan(0)
            .WithError(Errors.Validation.InvalidExpectedLength(nameof(GetLocationsQuery.Pagination.PageSize)))
            .LessThanOrEqualTo(100)
            .WithError(Errors.Validation.InvalidExpectedLength(nameof(GetLocationsQuery.Pagination.PageSize)));

        RuleFor(x => x.Search)
            .NotEmpty()
            .When(x => x.Search is not null)
            .WithError(Errors.Validation.CannotBeEmpty(nameof(GetLocationsQuery.Search)));

        RuleFor(x => x.MinDepartmentsCount)
            .GreaterThanOrEqualTo(0)
            .WithError(Errors.Validation.InvalidExpectedLength(nameof(GetLocationsQuery.MinDepartmentsCount)));

        RuleFor(x => x.SortBy)
            .Must(x => x.ToLowerInvariant() is "name" or "createdat" or "departmentscount")
            .WithError(Errors.General.ValueIsInvalid(nameof(GetLocationsQuery.SortBy)))
            .NotEmpty()
            .WithError(Errors.Validation.CannotBeEmpty(nameof(GetLocationsQuery.SortBy)));

        RuleFor(x => x.SortDirection)
            .Must(x => x.ToUpperInvariant() is "ASC" or "DESC")
            .WithError(Errors.General.ValueIsInvalid(nameof(GetLocationsQuery.SortDirection)));
    }
}