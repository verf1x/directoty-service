using DirectoryService.Domain.Shared;
using NJsonSchema.Generation;

namespace DirectoryService.Presentation.Extensions;

public class EnvelopeSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        if (context.Type != typeof(Envelope<ErrorList>))
            return;

        if (!context.Schema.Properties.TryGetValue("errors", out var errorsProperty))
            return;

        var errorSchema = context.Resolver.GetSchema(typeof(Error), isIntegerEnumeration: false);

        errorsProperty.Item = errorSchema;
    }
}