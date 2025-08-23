/*
 * Temporarily commenting out API versioning splitter during NSwag migration
 * TODO: Implement API versioning support for NSwag
 */

/*
using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using NSwag;
using NSwag.Generation;

namespace HttpApiDemo.Infrastructure;

/*
/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
internal sealed class SwaggerByApiVersionSplitter(IApiVersionDescriptionProvider provider) : IConfigureOptions<OpenApiDocumentGeneratorSettings>
{
    /// <inheritdoc />
    public void Configure(OpenApiDocumentGeneratorSettings settings)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
        {
            settings.PostProcess = document =>
            {
                document.Info = CreateInfoForApiVersion(description);
                document.Info.Title = "Demo API - " + description.GroupName;
            };
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder("Demo API");
        var info = new OpenApiInfo
        {
            Title = "Demo API - " + description.GroupName,
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact
            {
                Name = "Dennis Doomen",
                Email = "dennis.doomen@avivasolutions.nl"
            },
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        if (description.SunsetPolicy is SunsetPolicy policy)
        {
            if (policy.Date is DateTimeOffset when)
            {
                text.Append(" The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                for (var i = 0; i < policy.Links.Count; i++)
                {
                    var link = policy.Links[i];

                    if (link.Type == "text/html")
                    {
                        text.AppendLine();

                        if (link.Title.HasValue)
                        {
                            text.Append(link.Title.Value).Append(": ");
                        }

                        text.Append(link.LinkTarget.OriginalString);
                    }
                }
            }
        }

        info.Description = text.ToString();

        return info;
    }
}
*/
