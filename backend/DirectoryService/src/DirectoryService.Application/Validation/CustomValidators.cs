using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Validation;

public static class CustomValidators
{
    extension<T, TElement>(IRuleBuilder<T, TElement> ruleBuilder)
    {
        public IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<TValueObject>(
            Func<TElement, Result<TValueObject, Error>> factoryMethod) =>
            ruleBuilder.Custom((value, context) =>
            {
                Result<TValueObject, Error> result = factoryMethod(value);

                if (result.IsSuccess)
                    return;

                context.AddFailure(result.Error.Serialize());
            });
    }

    extension<T, TProperty>(IRuleBuilderOptions<T, TProperty> rule)
    {
        public IRuleBuilder<T, TProperty> WithError(Error error)
            => rule.WithMessage(error.Serialize());
    }
}