using System.Text.Json.Serialization;

namespace DirectoryService.Domain.Shared;

public record Error
{
    public string Code { get; }

    public string Message { get; }

    public ErrorType Type { get; }

    public string? InvalidField { get; }

    [JsonConstructor]
    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error NotFound(string? code, string message, Guid? id = null)
        => new Error(code ?? "record.not.found", message, ErrorType.NotFound);

    public static Error Validation(string? code, string message, string? invalidField = null)
        => new Error(code ?? "value.is.invalid", message, ErrorType.Validation, invalidField);

    public static Error Conflict(string? code, string message, string? invalidField = null)
        => new Error(code ?? "value.is.conflict", message, ErrorType.Conflict, invalidField);

    public static Error Failure(string? code, string message)
        => new Error(code ?? "failure", message, ErrorType.Failure);

    public ErrorsList ToErrors() => this;
}

public enum ErrorType
{
    /// <summary>
    /// Ошибка валидации
    /// </summary>
    Validation,

    /// <summary>
    /// Ошибка отсутствия ресурса
    /// </summary>
    NotFound,

    /// <summary>
    /// Общая ошибка
    /// </summary>
    Failure,

    /// <summary>
    /// Конфликт данных
    /// </summary>
    Conflict,
}