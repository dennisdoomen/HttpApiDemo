namespace HttpApiDemo;

public record VersionInfo
{
    /// <summary>
    /// The version string of the package (e.g., "1.2.3", "1.2.3-beta").
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// The description of this version of the package.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// The readme content or a link to the readme for this version.
    /// </summary>
    public required string Readme { get; init; }

    /// <summary>
    /// The URL to the license for this version.
    /// </summary>
    public required string LicenseUrl { get; init; }

    /// <summary>
    /// The license type or SPDX identifier for this version.
    /// </summary>
    public required string License { get; init; }

    /// <summary>
    /// The URL to the project site for this version.
    /// </summary>
    public required string ProjectUrl { get; init; }

    /// <summary>
    /// The URL to the icon for this version.
    /// </summary>
    public required string IconUrl { get; init; }

    /// <summary>
    /// The URL to the repository for this version.
    /// </summary>
    public required string RepositoryUrl { get; init; }

    /// <summary>
    /// The owner or maintainer of this version.
    /// </summary>
    public required string Owner { get; set; }
}
