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
        // Temporarily simplified - disable complex API versioning for initial NSwag migration
        // TODO: Re-implement API versioning support for NSwag
        
        // Add NSwag OpenAPI document generation
        builder.Services.AddOpenApiDocument(settings =>
        {
            settings.Title = "Demo API";
            settings.Version = "v1";
            
            var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            // TODO: Add XML documentation support for NSwag
            // if (File.Exists(filePath)) { ... }

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
        });
    }

    internal static void UseSwaggerUi(this WebApplication app)
    {
        // Use NSwag OpenAPI and Swagger UI
        app.UseOpenApi();
        app.UseSwaggerUi(settings =>
        {
            settings.Path = "/api-docs";
            settings.DocumentTitle = "Demo API's";
        });
    }

}
