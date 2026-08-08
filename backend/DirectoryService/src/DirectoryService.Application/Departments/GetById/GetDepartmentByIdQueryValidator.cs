using DirectoryService.Application.Validation;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Departments.GetById;

public class GetDepartmentByIdQueryValidator : AbstractValidator<GetDepartmentByIdQuery>
{
    public GetDepartmentByIdQueryValidator()
    {
        RuleFor(d => d.Id)
            .NotEmpty()
            .WithError(Errors.Validation.CannotBeEmpty(nameof(GetDepartmentByIdQuery.Id)));
    }
}