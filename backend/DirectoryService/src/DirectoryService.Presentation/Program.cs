using DirectoryService.Application;
using DirectoryService.Domain.Shared;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Presentation.Extensions;
using DirectoryService.Presentation.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfig) =>
{
    string seqUrl = context.Configuration.GetConnectionString("Seq")
                    ?? throw new InvalidOperationException("Seq connection string is not configured");

    loggerConfig.Configure(seqUrl);
});

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo { Title = "Directory Service API", Version = "v1", };

        return Task.CompletedTask;
    });

    options.AddSchemaTransformer((schema, context, _) =>
    {
        if (context.JsonTypeInfo.Type == typeof(Envelope<ErrorList>))
        {
            if (schema.Properties is not null
                && schema.Properties.TryGetValue("errors", out var errorsProperty)
                && errorsProperty is OpenApiSchema errorsSchema)
            {
                errorsSchema.Items = new OpenApiSchemaReference(nameof(Error), null, null);
            }
        }

        return Task.CompletedTask;
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionMiddleware();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Directory Service API")
            .WithOpenApiRoutePattern("/openapi/{documentName}.json");
    });
}

app.MapControllers();

app.Run();

public partial class Program;
