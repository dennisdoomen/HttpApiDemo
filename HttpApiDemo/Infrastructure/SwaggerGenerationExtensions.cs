using System.Text;
using System.Text.Json;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HttpApiDemo.Infrastructure;

internal static class SwaggerGenerationExtensions
{
    /// <summary>
    /// Registers API versioning, API Explorer, and one OpenAPI document per entry in
    /// <paramref name="apiVersionGroups"/>. Provide the groups explicitly before calling
    /// <c>builder.Build()</c> — minimal API endpoint versions are only discoverable after the
    /// application is built, so dynamic discovery via <c>BuildServiceProvider()</c> does not work.
    /// </summary>
    internal static void AddOpenApi(this WebApplicationBuilder builder, IReadOnlyList<ApiVersionGroup> apiVersionGroups)
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

        foreach (ApiVersionGroup apiVersionGroup in apiVersionGroups)
        {
            string groupName = apiVersionGroup.Name;

            builder.Services.AddOpenApi(groupName, options =>
            {
                options.AddDocumentTransformer((document, _, _) =>
                {
                    var text = new StringBuilder("Demo API");

                    if (apiVersionGroup.Deprecated)
                    {
                        text.Append(" This API version has been deprecated.");
                    }

                    document.Info = new OpenApiInfo
                    {
                        Title = "Demo API - " + groupName,
                        Version = apiVersionGroup.Version.ToString(),
                        Description = text.ToString(),
                        Contact = new OpenApiContact
                        {
                            Name = "Dennis Doomen",
                            Email = "dennis.doomen@avivasolutions.nl"
                        }
                    };

                    return Task.CompletedTask;
                });

                options.AddDocumentTransformer<SecuritySchemeTransformer>();

                options.AddOperationTransformer((operation, context, _) =>
                {
                    ApiDescription apiDescription = context.Description;
                    operation.Deprecated |= apiDescription.IsDeprecated;

                    PopulateParametersWithMissingMetadata(operation, apiDescription);

                    return Task.CompletedTask;
                });

                // Only include endpoints that belong to this API version group.
                options.ShouldInclude = api => api.GroupName == groupName;
            });
        }
    }

    internal static void UseOpenApiUi(this WebApplication app)
    {
        app.MapOpenApi("/api-docs/open-api-{documentName}.json");

        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();

            foreach (ApiVersionDescription description in descriptions.OrderBy(d => d.GroupName))
            {
                string url = $"/api-docs/open-api-{description.GroupName}.json";
                options.SwaggerEndpoint(url, description.GroupName);
            }

            options.RoutePrefix = "api-docs";
            options.DocumentTitle = "Demo API's";
        });
    }

    private static void PopulateParametersWithMissingMetadata(OpenApiOperation operation, ApiDescription apiDescription)
    {
        if (operation.Parameters is null)
        {
            return;
        }

        foreach (OpenApiParameter? parameter in operation.Parameters)
        {
            if (parameter is null)
            {
                continue;
            }

            ApiParameterDescription? description = apiDescription.ParameterDescriptions
                .FirstOrDefault(d => string.Equals(d.Name, parameter.Name, StringComparison.Ordinal));

            if (description is null)
            {
                continue;
            }

            parameter.Description ??= description.ModelMetadata?.Description;
            parameter.Required |= description.IsRequired;

            if (parameter.Schema is OpenApiSchema schema &&
                schema.Default == null &&
                description.DefaultValue != null &&
                description.DefaultValue is not DBNull &&
                description.ModelMetadata is ModelMetadata modelMetadata)
            {
                schema.Default = JsonSerializer.SerializeToNode(description.DefaultValue, modelMetadata.ModelType);
            }
        }
    }
}

