using System.Text.Json;
using System.Text.Json.Nodes;
using Asp.Versioning;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HttpApiDemo.Infrastructure;

/// <summary>
/// Applies workaround for know Swashbuckle issues and limitations.
/// </summary>
[UsedImplicitly]
internal sealed class ApplySwashbuckleWorkaroundsFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ApiDescription? apiDescription = context.ApiDescription;
        operation.Deprecated |= apiDescription.CustomAttributes().OfType<ObsoleteAttribute>().Any()
            || (apiDescription.ActionDescriptor.Properties.TryGetValue(typeof(ApiVersionModel), out var obj)
                && obj is ApiVersionModel model
                && model.DeprecatedApiVersions.Any());

        EnsureAllResponsesMatchSupportedContentTypes(operation, context);

        PopulateParametersWithMissingApiExplorerMetaData(operation, apiDescription);
    }

    /// <summary>
    /// Ensures that all responses in the OpenAPI operation match the supported content types defined in the API description.
    /// </summary>
    private static void EnsureAllResponsesMatchSupportedContentTypes(OpenApiOperation operation, OperationFilterContext context)
    {
        // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1752#issue-663991077
        foreach (ApiResponseType responseType in context.ApiDescription.SupportedResponseTypes)
        {
            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/b7cf75e7905050305b115dd96640ddd6e74c7ac9/src/Swashbuckle.AspNetCore.SwaggerGen/SwaggerGenerator/SwaggerGenerator.cs#L383-L387
            string responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();

            if (operation.Responses?.TryGetValue(responseKey, out var responseObject) == true &&
                responseObject is OpenApiResponse response &&
                response.Content != null)
            {
                foreach (string? contentType in response.Content.Keys)
                {
                    if (responseType.ApiResponseFormats.All(format => format.MediaType != contentType))
                    {
                        response.Content.Remove(contentType);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Adds the information that is available in the API explorer metadata to the generated swagger document.
    /// </summary>
    /// <remarks>
    /// This is necessary because of unresolved bug https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412 and
    /// proposed fix https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
    /// </remarks>
    private static void PopulateParametersWithMissingApiExplorerMetaData(OpenApiOperation operation, ApiDescription apiDescription)
    {
        if (operation.Parameters == null)
        {
            return;
        }

        foreach (OpenApiParameter? parameter in operation.Parameters)
        {
            if (parameter == null)
            {
                continue;
            }

            ApiParameterDescription description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema is OpenApiSchema concreteSchema &&
                concreteSchema.Default == null &&
                description.DefaultValue != null &&
                description.DefaultValue is not DBNull &&
                description.ModelMetadata is ModelMetadata modelMetadata)
            {
                // REF: https://github.com/Microsoft/aspnet-api-versioning/issues/429#issuecomment-605402330
                string json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
                concreteSchema.Default = JsonNode.Parse(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}
