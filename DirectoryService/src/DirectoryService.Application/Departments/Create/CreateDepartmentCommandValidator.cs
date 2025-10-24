using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Departments.Create;

public sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(x => x.Identifier)
            .MustBeValueObject(Identifier.Create);

        RuleFor(x => x.ParentId)
            .NotEmpty()
            .When(x => x.ParentId is not null);

        RuleFor(x => x.LocationIds)
            .Must(ids => ids.Length > 0)
            .WithError(Errors.Validation.InvalidLength(nameof(CreateDepartmentCommand.LocationIds), 1))
            .Must(locations => locations.Length == locations.Distinct().Count())
            .WithError(Errors.General.Conflict());
    }
}