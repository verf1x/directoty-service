using DirectoryService.Domain.Shared;
using FluentValidation.Results;

namespace DirectoryService.Application.Validation;

public static class ValidationExtensions
{
    extension(ValidationResult validationResult)
    {
        public ErrorList ToErrors()
        {
            var validationErrors = validationResult.Errors;

            var errors = from validationError in validationErrors
                let errorMessage = validationError.ErrorMessage
                let error = Error.Deserialize(errorMessage)
                select Error.Validation(error.Code, error.Message, validationError.PropertyName);

            return errors.ToArray();
        }
    }

    extension(IEnumerable<ValidationResult> validationResult)
    {
        public ErrorList ToErrors()
            => validationResult.SelectMany(e => e.Errors)
                .Select(e => Error.Validation(e.ErrorCode, e.ErrorMessage, e.PropertyName))
                .ToArray();
    }
}