using FluentValidation;

namespace DirectoryService.Application.Locations.GetById;

public class GetLocationByIdQueryValidator : AbstractValidator<GetLocationByIdQuery>
{
    public GetLocationByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}