using DirectoryService.Application.Validation;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Departments.UpdateLocations;

public class UpdateDepartmentLocationsCommandValidator : AbstractValidator<UpdateDepartmentLocationsCommand>
{
    public UpdateDepartmentLocationsCommandValidator()
    {
        RuleFor(ud => ud.DepartmentId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsInvalid(nameof(UpdateDepartmentLocationsCommand)));

        RuleFor(x => x.LocationIds)
            .Must(ids => ids.Length > 0)
            .WithError(Errors.Validation.InvalidLength(nameof(UpdateDepartmentLocationsCommand.LocationIds), 1))
            .Must(locations => locations.Length == locations.Distinct().Count())
            .WithError(Errors.General.Conflict());
    }
}