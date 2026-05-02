using Asp.Versioning;

namespace HttpApiDemo.Infrastructure;

/// <summary>
/// Describes a single API version group (e.g., "public" × v2.0) whose OpenAPI document should be generated.
/// Declare all groups before calling <c>builder.AddOpenApi()</c> so their documents can be registered
/// with <see cref="Microsoft.AspNetCore.Builder.OpenApiEndpointRouteBuilderExtensions.MapOpenApi"/> before
/// <c>builder.Build()</c> is called.
/// </summary>
internal record ApiVersionGroup(string Group, ApiVersion Version, bool Deprecated = false)
{
    /// <summary>
    /// The formatted document name, e.g. "public-v2" or "public-v0.1".
    /// Matches <c>options.GroupNameFormat = "'v'VVV"</c> and <c>options.FormatGroupName = (g, v) => $"{g}-{v}"</c>.
    /// </summary>
    public string Name => $"{Group}-{Version.ToString("'v'VVV")}";
}
