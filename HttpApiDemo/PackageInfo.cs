namespace HttpApiDemo;

public record PackageInfo
{
    /// <summary>
    /// Gets or sets the unique identifier for a package.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the total number of downloads for the package across all versions.
    /// </summary>
    public int TotalDownloads { get; init; }

    /// <summary>
    /// Gets or sets the collection of available versions for a package,
    /// including related metadata such as downloads, descriptions, and URLs.
    /// </summary>
    public IEnumerable<VersionInfo> Versions { get; init; } = [];
}
