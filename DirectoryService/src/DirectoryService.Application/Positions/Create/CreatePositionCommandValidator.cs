using DirectoryService.Application.Validation;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Positions.Create;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(cp => cp.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(cp => cp.Description)
            .MustBeValueObject(Description.Create!)
            .When(cp => !string.IsNullOrWhiteSpace(cp.Description));

        RuleFor(cp => cp.DepartmentIds)
            .Must(ids => ids.Length > 0)
            .WithError(Errors.Validation.InvalidLength(nameof(CreatePositionCommand.DepartmentIds), 1))
            .Must(positions => positions.Length == positions.Distinct().Count())
            .WithError(Errors.General.Conflict());
    }
}