using DirectoryService.Application.Validation;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Locations.Get;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(x => x.Pagination.PageNumber)
            .GreaterThan(0)
            .WithError(Errors.Validation.InvalidExpectedLength(nameof(GetLocationsQuery.Pagination.PageNumber)));

        RuleFor(x => x.Pagination.PageSize)
            .GreaterThan(0)
            .WithError(Errors.Validation.InvalidExpectedLength(nameof(GetLocationsQuery.Pagination.PageSize)));

        RuleFor(x => x.Search)
            .NotEmpty()
            .When(x => x.Search is not null);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .When(x => x.PostalCode is not null)
            .Length(6, 6)
            .When(x => x.PostalCode is not null);

        RuleFor(x => x.Region)
            .NotEmpty()
            .When(x => x.Region is not null);

        RuleFor(x => x.City)
            .NotEmpty()
            .When(x => x.City is not null);

        RuleFor(x => x.District)
            .NotEmpty()
            .When(x => x.District is not null);

        RuleFor(x => x.Street)
            .NotEmpty()
            .When(x => x.Street is not null);

        RuleFor(x => x.House)
            .NotEmpty()
            .When(x => x.House is not null);

        RuleFor(x => x.Building)
            .NotEmpty()
            .When(x => x.Building is not null);

        RuleFor(x => x.Apartment)
            .NotEmpty()
            .When(x => x.Apartment is not null);

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .When(x => x.TimeZone is not null);

        RuleFor(x => x.SortBy)
            .NotEmpty()
            .WithError(Errors.Validation.CannotBeEmpty(nameof(GetLocationsQuery.SortBy)));

        RuleFor(x => x.SortDirection)
            .Must(x => x.ToUpperInvariant() is "ASC" or "DESC")
            .WithError(Errors.General.ValueIsInvalid(nameof(GetLocationsQuery.SortDirection)));
    }
}