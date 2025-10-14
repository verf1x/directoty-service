using DirectoryService.Domain;
using FluentValidation.Results;

namespace DirectoryService.Application.Extensions;

public static class ValidationExtensions
{
    public static ErrorsList ToErrors(this ValidationResult validationResult)
        => validationResult.Errors
            .Select(e => Error.Validation(e.ErrorCode, e.ErrorMessage, e.PropertyName))
            .ToArray();

    public static ErrorsList ToErrors(this IEnumerable<ValidationResult> validationResult)
        => validationResult.SelectMany(e => e.Errors)
            .Select(e => Error.Validation(e.ErrorCode, e.ErrorMessage, e.PropertyName))
            .ToArray();
}