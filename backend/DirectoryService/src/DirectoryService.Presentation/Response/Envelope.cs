using System.Text.Json.Serialization;
using DirectoryService.Domain;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Presentation.Response;

public record Envelope
{
    public object? Result { get; }

    public ErrorList? Errors { get; }

    public DateTime CreationDate { get; }

    [JsonConstructor]
    private Envelope(object? result, ErrorList? errors)
    {
        Result = result;
        Errors = errors;
        CreationDate = DateTime.Now;
    }

    public static Envelope Ok(object? result = null)
        => new(result, null);

    public static Envelope Error(ErrorList errors)
        => new(null, errors);
}