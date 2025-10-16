using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations;
using FluentValidation;
using TimeZone = DirectoryService.Domain.Locations.TimeZone;

namespace DirectoryService.Application.Locations.Create;

public sealed class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(c => c)
            .MustBeValueObject(a => Address.Create(
                a.PostalCode,
                a.Region,
                a.City,
                a.District,
                a.Street,
                a.House,
                a.Building,
                a.Apartment));

        RuleFor(c => c.TimeZone)
            .MustBeValueObject(TimeZone.Create);
    }
}