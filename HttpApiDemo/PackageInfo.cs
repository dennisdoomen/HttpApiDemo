using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HttpApiDemo;

public record PackageInfo
{
    [Description("The unique identifier for a package")]
    public required string Id { get; init; }

    [Description("The total number of downloads for the package across all versions")]
    public int TotalDownloads { get; init; }

    [Description(
        "The collection of available versions for a package, including related metadata such as downloads, descriptions, and URLs")]
    public IEnumerable<VersionInfo> Versions { get; init; } = [];

    /// <summary>
    /// Imaginary ID to demonstrate asynchronous processing of new packages.
    /// </summary>
    [JsonIgnore]
    public string? PendingId { get; set; }

    /// <summary>
    /// Imaginary state to demonstrate asynchronous processing of new packages.
    /// </summary>
    [JsonIgnore]
    public bool IsPending = false;
}
