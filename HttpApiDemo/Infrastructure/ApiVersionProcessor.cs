using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Asp.Versioning;

namespace HttpApiDemo.Infrastructure;

/// <summary>
/// Filters operations based on API version for NSwag documentation generation.
/// </summary>
public class ApiVersionProcessor : IOperationProcessor
{
    private readonly string _version;

    public ApiVersionProcessor(string version)
    {
        _version = version;
    }

    public bool Process(OperationProcessorContext context)
    {
        // Check if this operation has version mapping
        var mapToVersionAttributes = context.MethodInfo.GetCustomAttributes(typeof(MapToApiVersionAttribute), false)
            .OfType<MapToApiVersionAttribute>();
        
        if (mapToVersionAttributes.Any())
        {
            var mappedVersions = mapToVersionAttributes
                .SelectMany(attr => attr.Versions)
                .Select(v => v.ToString());
                
            return mappedVersions.Contains(_version);
        }
        
        // If no explicit version mapping, exclude from all versioned docs
        return false;
    }
}