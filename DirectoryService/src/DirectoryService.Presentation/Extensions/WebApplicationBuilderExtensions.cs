namespace DirectoryService.Presentation.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static string GetSeqConnectionString(this WebApplicationBuilder builder) =>
        builder.Configuration.GetConnectionString("Seq")
        ?? throw new ArgumentNullException("Seq");
}