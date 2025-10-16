namespace DirectoryService.Domain;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            string label = name ?? "value";
            return Error.Validation("value.is.invalid", $"{label} is invalid", label);
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
}