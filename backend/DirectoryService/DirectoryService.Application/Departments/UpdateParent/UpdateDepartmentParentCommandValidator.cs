using DirectoryService.Application.Validation;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Departments.UpdateParent;

public sealed class UpdateDepartmentParentCommandValidator : AbstractValidator<UpdateDepartmentParentCommand>
{
    public UpdateDepartmentParentCommandValidator()
    {
        RuleFor(udp => udp.DepartmentId)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(UpdateDepartmentParentCommand.DepartmentId)));

        RuleFor(udp => udp.ParentId)
            .NotEmpty()
            .When(udp => udp.ParentId is not null)
            .WithError(Errors.General.ValueIsInvalid(nameof(UpdateDepartmentParentCommand.ParentId)));

        RuleFor(udp => udp)
            .Must(udp => udp.ParentId != udp.DepartmentId)
            .WithError(Errors.General.Conflict());
    }
}