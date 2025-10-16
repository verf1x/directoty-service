using FluentValidation;

namespace DirectoryService.Application.Locations.Create;

public sealed class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .Length(3, 120);

        RuleFor(c => c.PostalCode)
            .Length(6);

        RuleFor(c => c.Region)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(c => c.City)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(c => c.District)
            .MaximumLength(100)
            .When(c => c.District is not null);

        RuleFor(c => c.Street)
            .MaximumLength(250)
            .NotEmpty();

        RuleFor(c => c.House)
            .MaximumLength(10)
            .NotEmpty();

        RuleFor(c => c.Building)
            .MaximumLength(10)
            .When(c => c.Building is not null);

        RuleFor(c => c.Apartment)
            .MaximumLength(10)
            .When(c => c.Apartment is not null);

        RuleFor(c => c.TimeZone)
            .NotEmpty()
            .Must(tz => TimeZoneInfo.TryFindSystemTimeZoneById(tz, out _));
    }
}