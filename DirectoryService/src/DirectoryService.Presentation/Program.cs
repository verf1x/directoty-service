using DirectoryService.Application;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Presentation.Extensions;
using DirectoryService.Presentation.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SchemaType = NJsonSchema.SchemaType;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfig) =>
{
    string seqUrl = context.Configuration.GetConnectionString("Seq")
                    ?? throw new InvalidOperationException("Seq connection string is not configured");

    loggerConfig.Configure(seqUrl);
});

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "Directory Service API";
    settings.Version = "v1";

    settings.SchemaSettings.SchemaType = SchemaType.OpenApi3;

    settings.SchemaSettings.GenerateEnumMappingDescription = true;

    settings.SchemaSettings.SchemaProcessors.Add(new EnvelopeSchemaProcessor());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionMiddleware();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

public partial class Program;