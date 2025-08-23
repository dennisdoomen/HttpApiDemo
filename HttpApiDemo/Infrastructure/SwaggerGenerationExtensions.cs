using Asp.Versioning;

namespace HttpApiDemo.Infrastructure;

internal static class SwaggerGenerationExtensions
{
    internal static void AddOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.UnsupportedApiVersionStatusCode = 404; // NotFound
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.FormatGroupName = (group, version) => $"{group}-{version}";
                options.SubstitutionFormat = "V";
                options.SubstituteApiVersionInUrl = true;
            });

        // Add .NET 9 built-in OpenAPI support
        builder.Services.AddOpenApi();

        // Add endpoints API explorer for controller discovery
        builder.Services.AddEndpointsApiExplorer();

        // Add NSwag services for enhanced documentation and UI
        builder.Services.AddOpenApiDocument(configure =>
        {
            configure.Title = "Demo API";
            configure.Version = "v1";
            configure.DocumentName = "v1";
            
            // Include XML comments
            var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(filePath))
            {
                configure.PostProcess = document =>
                {
                    document.Info.Title = "Demo API";
                    document.Info.Version = "v1";
                };
            }
        });
    }

    internal static void UseOpenApiUi(this WebApplication app)
    {
        // Use .NET 9 built-in OpenAPI
        app.MapOpenApi();
        
        // Use NSwag UI for better documentation experience
        app.UseOpenApi();
        app.UseSwaggerUi(settings =>
        {
            settings.Path = "/api-docs";
            settings.DocumentPath = "/swagger/v1/swagger.json";
        });
    }
}
