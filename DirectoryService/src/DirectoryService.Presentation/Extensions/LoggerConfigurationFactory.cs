using Serilog;
using Serilog.Events;

namespace DirectoryService.Presentation.Extensions;

public static class LoggerConfigurationFactory
{
    extension(LoggerConfiguration configuration)
    {
        public void Configure(string seqConnection)
        {
            configuration
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.Seq(seqConnection)
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Warning);
        }
    }
}