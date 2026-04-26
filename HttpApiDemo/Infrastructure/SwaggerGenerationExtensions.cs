using Asp.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
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

    internal static void UseSwaggerUi(this WebApplication app)
    {
        app.UseSwagger(options => { options.RouteTemplate = "api-docs/{version}/open-api-{documentName}.json"; });

            var descriptions = app.DescribeApiVersions();
            var sortedDescriptions = descriptions.OrderBy(description => description.GroupName);

            // Build a ReDoc page for every API version (and group) as ReDoc does not support multiple API versions in an integrated UI
            foreach (var description in sortedDescriptions)
            {
                app.UseReDoc(options =>
                {
                    var url = $"/api-docs/v{description.ApiVersion.MajorVersion}/open-api-{description.GroupName}.json";
                    var name = description.GroupName;
                    options.RoutePrefix = "api-docs/" + name;
                    options.SpecUrl = url;
                    options.DocumentTitle = name;
                });
            }

            // Redirect the root to the API docs index page
            app.MapGet("/", () => Results.Redirect("/api-docs/index.html"));

            // Serve a simple index page listing all available API versions
            var links = sortedDescriptions
                .Select(d => $"<li><a href='/api-docs/{d.GroupName}'>{d.GroupName}</a></li>");

            app.MapGet("/api-docs/index.html", () => Results.Content(
                $"""
                <!DOCTYPE html>
                <html>
                <head><title>API Documentation</title></head>
                <body>
                  <h1>API Documentation</h1>
                  <ul>{string.Join("", links)}</ul>
                </body>
                </html>
                """, "text/html"));
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
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Basic", document)] = [],
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    }
}
