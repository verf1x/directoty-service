using FluentValidation;

namespace DirectoryService.Application.Positions.GetById;

public class GetPositionByIdQueryValidator : AbstractValidator<GetPositionByIdQuery>
{
    public GetPositionByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}