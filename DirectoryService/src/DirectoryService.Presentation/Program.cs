using DirectoryService.Application;
using DirectoryService.Domain;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer((schema, context, _) =>
    {
        if (context.JsonTypeInfo.Type == typeof(Envelope<ErrorList>))
        {
            if (schema.Properties.TryGetValue("errors", out var errorsProperty))
            {
                errorsProperty.Items.Reference = new OpenApiReference
                {
                    Type = ReferenceType.Schema, Id = nameof(Error),
                };
            }
        }

        return Task.CompletedTask;
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Directory Service API"));
}

app.MapControllers();

app.Run();