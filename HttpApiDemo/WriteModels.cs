using System.ComponentModel;

namespace HttpApiDemo;

public record PutPackageRequest
{
    [Description("The total number of downloads for the package across all versions")]
    public required int TotalDownloads { get; init; }

    [Description("The versions that belong to this package registration")]
    public required IEnumerable<VersionInfo> Versions { get; init; } = Array.Empty<VersionInfo>();
}

public record PatchPackageRequest
{
    [Description("Optional replacement for the total number of downloads")]
    public int? TotalDownloads { get; init; }

    [Description("Optional replacement of the entire versions list. When provided, it replaces the full set.")]
    public IEnumerable<VersionInfo>? Versions { get; init; }
}
