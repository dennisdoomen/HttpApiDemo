using System.ComponentModel;

namespace HttpApiDemo;

public record VersionInfo
{
    [Description("The version string of the package (e.g., \"1.2.3\", \"1.2.3-beta\")")]
    public required string Version { get; init; }

    [Description("The description of this version of the package")]
    public required string Description { get; init; }

    [Description("The readme content or a link to the readme for this version")]
    public required string Readme { get; init; }

    [Description("The URL to the license for this version")]
    public required string LicenseUrl { get; init; }

    [Description("The license type or SPDX identifier for this version")]
    public required string License { get; init; }

    [Description("The URL to the project site for this version")]
    public required string ProjectUrl { get; init; }

    [Description("The URL to the icon for this version")]
    public required string IconUrl { get; init; }

    [Description("The URL to the repository for this version")]
    public required string RepositoryUrl { get; init; }

    [Description("The owner or maintainer of this version")]
    public required string Owner { get; set; }
}
