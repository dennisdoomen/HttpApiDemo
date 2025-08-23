using Asp.Versioning;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Contexts;
using NSwag;
using NSwag.Generation;

namespace HttpApiDemo.Infrastructure;

internal static class SwaggerGenerationExtensions
{
    internal static void AddSwaggerGen(this WebApplicationBuilder builder)
    {
        // Add API versioning support
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

        // Add NSwag OpenAPI document generation with versioning support
        builder.Services.AddOpenApiDocument(settings =>
        {
            settings.DocumentName = "v1";
            settings.Title = "Demo API - v1";
            settings.Version = "1.0";
            
            // Filter to include only v1 operations
            settings.OperationProcessors.Add(new ApiVersionProcessor("1.0"));
            
            var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            // TODO: Add XML documentation support for NSwag
            // if (File.Exists(filePath)) { ... }

            AddSecurityDefinitions(settings);
        });

        builder.Services.AddOpenApiDocument(settings =>
        {
            settings.DocumentName = "v2";
            settings.Title = "Demo API - v2";
            settings.Version = "2.0";
            
            // Filter to include only v2 operations
            settings.OperationProcessors.Add(new ApiVersionProcessor("2.0"));
            
            var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            AddSecurityDefinitions(settings);
        });
    }

    private static void AddSecurityDefinitions(OpenApiDocumentGeneratorSettings settings)
    {
        // Add security definitions for Bearer token
        settings.AddSecurity("Bearer", [], new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
        });

        // Add security definitions for Basic auth
        settings.AddSecurity("Basic", [], new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.Http,
            Scheme = "Basic",
            Description = "Basic Authentication using the HTTP Basic scheme. Provide a Base64-encoded 'username:password' " +
                          "in the Authorization header. Example: \"Authorization: Basic dXNlcm5hbWU6cGFzc3dvcmQ=\""
        });
    }

    internal static void UseSwaggerUi(this WebApplication app)
    {
        // Use NSwag OpenAPI and Swagger UI with versioning
        app.UseOpenApi();
        app.UseSwaggerUi(settings =>
        {
            settings.Path = "/api-docs";
            settings.DocumentTitle = "Demo API's";
            
            // Add v1 and v2 endpoints to the dropdown
            settings.SwaggerRoutes.Add(new SwaggerUiRoute("v1.0", "/swagger/v1/swagger.json"));
            settings.SwaggerRoutes.Add(new SwaggerUiRoute("v2.0", "/swagger/v2/swagger.json"));
        });
    }

}
