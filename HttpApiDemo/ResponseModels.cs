using System.ComponentModel;

namespace HttpApiDemo;

public record PackageResponse
{
    [Description("The unique identifier for a package")]
    public required string Id { get; init; }

    [Description("A short functional description of the package")]
    public required string Description { get; init; }
}

public record VersionSummary
{
    [Description("The version string of the package (e.g., '1.2.3')")]
    public required string Version { get; init; }

    [Description("The description of this version of the package")]
    public required string Description { get; init; }

    [Description("The URL to the repository for this version")]
    public required string RepositoryUrl { get; init; }

    [Description("The owner or maintainer of this version")]
    public required string Owner { get; init; }
}

public record PackageWithVersionSummaryResponse
{
    [Description("The unique identifier for a package")]
    public required string Id { get; init; }

    [Description("The versions of the package with basic metadata (version, description, repository and owner)")]
    public required IEnumerable<VersionSummary> Versions { get; init; } = [];
}

public record VersionDetails
{
    [Description("The version string of the package (e.g., '1.2.3')")]
    public required string Version { get; init; }

    [Description("The description of this version of the package")]
    public required string Description { get; init; }

    [Description("The readme content or a link to the readme for this version")]
    public required string Readme { get; init; }

    [Description("The URL to the license for this version")]
    public required string LicenseUrl { get; init; }

    [Description("The URL to the project site for this version")]
    public required string ProjectUrl { get; init; }

    [Description("The URL to the icon for this version")]
    public required string IconUrl { get; init; }

    [Description("The URL to the repository for this version")]
    public required string RepositoryUrl { get; init; }

    [Description("The owner or maintainer of this version")]
    public required string Owner { get; init; }
}

public record PackageWithVersionDetailsResponse
{
    [Description("The unique identifier for a package")]
    public required string Id { get; init; }

    [Description(
        "The versions of the package with extended metadata (including readme, license, project, icon, repository and owner)")]
    public required IEnumerable<VersionDetails> Versions { get; init; } = [];
}

public record PackageStatistics
{
    [Description("The unique identifier for a package")]
    public required string Id { get; init; }

    [Description("The total number of downloads for the package across all versions")]
    public required int TotalDownloads { get; init; }
}

