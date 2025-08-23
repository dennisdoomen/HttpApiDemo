using Asp.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HttpApiDemo.Infrastructure;

internal static class SwaggerGenerationExtensions
{
    internal static void AddSwaggerGen(this WebApplicationBuilder builder)
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

        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerByApiVersionSplitter>();
        builder.Services.AddSwaggerGen(options =>
        {
            // Add a custom operation filter which sets default values
            options.OperationFilter<ApplySwashbuckleWorkaroundsFilter>();

            var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            // Integrate xml comments
            options.IncludeXmlComments(filePath);

            AddSecurityDefinitions(options);
            AddSecurityRequirements(options);
        });
    }

    internal static void UseScalar(this WebApplication app)
    {
        // Use default Swagger/OpenAPI generation (not custom route template)
        app.UseSwagger();

        var descriptions = app.DescribeApiVersions();
        var sortedDescriptions = descriptions.OrderBy(description => description.GroupName);

        // Configure Scalar to use the same route as original SwaggerUI (/api-docs)
        if (sortedDescriptions.Any())
        {
            var defaultDescription = sortedDescriptions.First();
            // Use default swagger pattern: /swagger/{documentName}/swagger.json
            var openApiUrl = $"/swagger/{defaultDescription.GroupName}/swagger.json";

            // Map Scalar at /api-docs (exactly where SwaggerUI was)
            app.MapGet("/api-docs", () => Results.Content(
                GetScalarHtml(openApiUrl, "Demo API's"), 
                "text/html"))
                .WithName("scalar-ui")
                .ExcludeFromDescription();
        }
    }

    private static string GetScalarHtml(string openApiUrl, string title)
    {
        return $@"<!DOCTYPE html>
<html>
  <head>
    <title>{title}</title>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  </head>
  <body>
    <script
      id=""api-reference""
      type=""application/json""
      data-url=""{openApiUrl}""
    >
    </script>
    <script src=""https://cdn.jsdelivr.net/npm/@scalar/api-reference""></script>
  </body>
</html>";
    }

    private static void AddSecurityDefinitions(SwaggerGenOptions options)
    {
        // Add an Authorize button to enable bearer token authentication
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });

        // Add an Authorize button to enable basic authentication
        options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
        {
            Description = "Basic Authentication using the HTTP Basic scheme. Provide a Base64-encoded 'username:password' " +
                          "in the Authorization header. Example: \"Authorization: Basic dXNlcm5hbWU6cGFzc3dvcmQ=\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
        });
    }

    private static void AddSecurityRequirements(SwaggerGenOptions options)
    {
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Basic"
                    }
                },
                []
            },
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                []
            }
        });
    }
}
