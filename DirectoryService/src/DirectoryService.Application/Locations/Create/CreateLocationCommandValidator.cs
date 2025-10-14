using FluentValidation;

namespace DirectoryService.Application.Locations.Create;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .Length(3, 120);

        RuleFor(c => c.AddressLines)
            .Must(al => al.Count() is >= 2 and <= 4);

        RuleFor(c => c.Locality)
            .NotEmpty();

        RuleFor(c => c.Region)
            .NotEmpty()
            .When(c => c.Region is not null);

        RuleFor(c => c.PostalCode)
            .NotEmpty()
            .When(c => c.PostalCode is not null);

        RuleFor(c => c.CountryCode)
            .NotEmpty()
            .Matches("^[A-Z]{2}$");

        RuleFor(c => c.TimeZone)
            .NotEmpty()
            .Must(tz => TimeZoneInfo.TryFindSystemTimeZoneById(tz, out _));
    }
}