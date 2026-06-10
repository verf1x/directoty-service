#pragma warning disable SA1137

namespace DirectoryService.Presentation.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public string GetSeqConnectionString() =>
            builder.Configuration.GetConnectionString("Seq")
            ?? throw new ArgumentNullException("Seq");
    }
}