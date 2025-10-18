namespace DirectoryService.Domain.Shared;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            string label = name ?? "value";
            return Error.Validation("value.is.invalid", $"{label} is invalid", label);
        }

        public static Error ValueIsInvalid(string name, string message)
        {
            return Error.Validation("value.is.invalid", message, name);
        }

        public static Error NotFound(Guid? id = null)
        {
            string forId = id is null ? string.Empty : $" for id '{id}'";
            return Error.NotFound("record.not.found", $"record not found{forId}");
        }

        public static Error ValueIsRequired(string? name = null)
        {
            string label = name is null ? " " : " " + name + " ";
            return Error.NotFound("value.is.required", $"invalid{label}length");
        }

        public static Error Conflict(Guid? id = null)
        {
            string forId = id is null ? string.Empty : $" for id '{id}'";
            return Error.NotFound("value.already.exists", $"value already exists{forId}");
        }
    }

    public static class Validation
    {
        public static Error CannotBeNullOrEmpty(string? name = null)
        {
            string label = name ?? "value";
            return Error.Validation("value.is.null.or.empty", $"{label} cannot be null or empty", label);
        }

        public static Error CannotBeEmpty(string? name = null)
        {
            string label = name ?? "value";
            return Error.Validation("value.is.empty", $"{label} cannot be empty", label);
        }

        public static Error InvalidLength(string? name = null, int? minLength = null, int? maxLength = null)
        {
            string label = name ?? "value";

            if (minLength is not null && maxLength is not null)
            {
                return Error.Validation(
                    "value.length.is.invalid",
                    $"{label} length must be between {minLength} and {maxLength}",
                    label);
            }

            if (minLength is not null && maxLength is null)
            {
                return Error.Validation(
                    "value.length.is.invalid",
                    $"{label} length must be at least {minLength}",
                    label);
            }

            if (minLength is null && maxLength is not null)
            {
                return Error.Validation(
                    "value.length.is.invalid",
                    $"{label} length must be at most {maxLength}",
                    label);
            }

            return Error.Validation("value.length.is.invalid", $"{label} length is invalid", label);
        }

        public static Error InvalidExpectedLength(string? name = null, int? expectedLength = null)
        {
            string label = name ?? "value";

            if (expectedLength is not null)
            {
                return Error.Validation(
                    "value.length.is.invalid",
                    $"{label} length must be {expectedLength}",
                    label);
            }

            return Error.Validation("value.length.is.invalid", $"{label} length is invalid", label);
        }
    }
}