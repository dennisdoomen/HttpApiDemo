/*
 * This file contains Swashbuckle-specific workarounds that may not be needed with NSwag.
 * Commenting out for now during migration to NSwag.
 * TODO: Review if any of these workarounds are still needed with NSwag.
 */

/*
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HttpApiDemo.Infrastructure;

/*
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
        operation.Deprecated |= apiDescription.IsDeprecated();

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
            OpenApiResponse? response = operation.Responses[responseKey];

            foreach (string? contentType in response.Content.Keys)
            {
                if (responseType.ApiResponseFormats.All(format => format.MediaType != contentType))
                {
                    response.Content.Remove(contentType);
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
            ApiParameterDescription description = apiDescription.ParameterDescriptions.First(description => description.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null &&
                description.DefaultValue != null &&
                description.DefaultValue is not DBNull &&
                description.ModelMetadata is ModelMetadata modelMetadata)
            {
                // REF: https://github.com/Microsoft/aspnet-api-versioning/issues/429#issuecomment-605402330
                string json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}
*/
