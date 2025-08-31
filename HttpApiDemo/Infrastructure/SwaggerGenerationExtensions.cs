using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace HttpApiDemo.Infrastructure;

internal static class SwaggerGenerationExtensions
{
    internal static void AddOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.UnsupportedApiVersionStatusCode = 404; // NotFound
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("api-version"));
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.FormatGroupName = (group, version) => $"{group}-{version}";
                options.SubstitutionFormat = "V";
                options.SubstituteApiVersionInUrl = true;
            });

        var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (string group in provider.ApiVersionDescriptions.Select(x => x.GroupName))
        {
            // Add .NET 9 built-in OpenAPI support
            builder.Services.AddOpenApi(group, options =>
            {
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Info.Title = "HTTP Api OpenAPi Demo";
                    document.Info.Description = "This API demonstrates OpenAPI customization in a .NET 9 project.";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "Dennis Doomen",
                        Email = "mukesh@codewithmukesh.com",
                        Url = new Uri("https://codewithmukesh.com")
                    };

                    document.Info.License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    };

                    return Task.CompletedTask;
                });

                // Indicate if the API is deprecated. This is somehow not picekd up automatically right now.
                options.AddOperationTransformer((operation, context, _) =>
                {
                    var apiDescription = context.Description;
                    operation.Deprecated = apiDescription.IsDeprecated();
                    return Task.CompletedTask;
                });

                // Selecting which APIs belong to this group
                options.ShouldInclude = api => api.GroupName == options.DocumentName;
            });
        }
    }

    internal static void UseOpenApiUi(this WebApplication app)
    {
        // Use .NET 9 built-in OpenAPI
        app.MapOpenApi("/api-docs/open-api-{documentName}.json");

        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("v1 Penguins API")
                .WithTheme(ScalarTheme.DeepSpace)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

            var descriptions = app.DescribeApiVersions();
            var sortedDescriptions = descriptions.OrderBy(description => description.GroupName);

            // Build a swagger endpoint for each discovered API version
            foreach (var description in sortedDescriptions)
            {
                var url = $"/api-docs/open-api-{description.GroupName}.json";
                var name = description.GroupName;

                options.AddDocument(name, name, url);
            }
        });
    }
}
