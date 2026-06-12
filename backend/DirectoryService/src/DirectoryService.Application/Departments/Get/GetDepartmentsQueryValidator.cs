using DirectoryService.Application.Validation;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Departments.Get;

public class GetDepartmentsQueryValidator : AbstractValidator<GetDepartmentsQuery>
{
    public GetDepartmentsQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithError(Errors.Validation.InvalidLength(nameof(GetDepartmentsQuery.Search), maxLength: 100));

        RuleFor(x => x.SortBy)
            .Must(x => x is null or "name" or "created_at")
            .WithError(Errors.General.ValueIsInvalid(nameof(GetDepartmentsQuery.SortBy)));

        RuleFor(x => x.SortDirection)
            .Must(x => x is null
                || string.Equals(x, "asc", StringComparison.OrdinalIgnoreCase)
                || string.Equals(x, "desc", StringComparison.OrdinalIgnoreCase))
            .WithError(Errors.General.ValueIsInvalid(nameof(GetDepartmentsQuery.SortDirection)));

        RuleFor(x => x.Pagination.Page)
            .GreaterThan(0)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetDepartmentsQuery.Pagination.Page)));

        RuleFor(x => x.Pagination.PageSize)
            .LessThanOrEqualTo(100)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetDepartmentsQuery.Pagination.PageSize)));
    }
}
