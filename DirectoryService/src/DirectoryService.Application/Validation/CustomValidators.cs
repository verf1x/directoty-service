using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Validation;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Error>> factoryMethod) =>
        ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject, Error> result = factoryMethod(value);

            if (result.IsSuccess)
                return;

            context.AddFailure(result.Error.Serialize());
        });

    public static IRuleBuilder<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Error error)
        => rule.WithMessage(error.Serialize());
}