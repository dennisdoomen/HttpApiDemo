namespace HttpApiDemo;

public class PackageRepository : IRequirePackageInformation
{
    public Task<PackageInfo?> FindPackageInfo(string packageId)
    {
        return Task.FromResult(examplePackages.FirstOrDefault(x => x.Id.Equals(packageId, StringComparison.CurrentCultureIgnoreCase)));
    }

    public Task<IEnumerable<(string Id, string Description)>> GetPackageList() => Task.FromResult(examplePackages
        .Select(x => (x.Id, x.Versions.FirstOrDefault()?.Description ?? string.Empty))
        .AsEnumerable());

    private readonly IEnumerable<PackageInfo> examplePackages =
    [
        new()
        {
            Id = "FluentAssertions",
            TotalDownloads = 538_494_255,
            Versions = new[]
            {
                new VersionInfo
                {
                    Version = "8.6.0", // Most recent 8.x release
                    Description =
                        "A very extensive set of extension methods that allow you to more naturally specify the expected outcome of a TDD or BDD-style unit tests.",
                    Readme = "See Fluent Assertions documentation on NuGet.",
                    LicenseUrl =
                        "https://opensource.org/licenses/Apache-2.0 (prior to v8); new license info available at the FluentAssertions site.",
                    License = "Commercial (for v8+, non-commercial open-source free)",
                    ProjectUrl = "https://github.com/fluentassertions/fluentassertions",
                    IconUrl = "", // Not always provided on NuGet
                    RepositoryUrl = "https://github.com/fluentassertions/fluentassertions",
                    Owner = "dennisdoomen"
                },
                new VersionInfo
                {
                    Version = "7.0.0", // Most recent 7.x branch
                    Description = "Same assertion library, last fully open-source version under Apache-2.0 license.",
                    Readme = "See Fluent Assertions documentation on NuGet.",
                    LicenseUrl = "https://opensource.org/licenses/Apache-2.0",
                    License = "Apache-2.0",
                    ProjectUrl = "https://github.com/fluentassertions/fluentassertions",
                    IconUrl = "",
                    RepositoryUrl = "https://github.com/fluentassertions/fluentassertions",
                    Owner = "dennisdoomen"
                },
            }
        },
        new()
        {
            Id = "PackageGuard",
            TotalDownloads = 3484,
            Versions = new[]
            {
                new VersionInfo
                {
                    Version = "1.5.0", // Most recent known
                    Description =
                        "PackageGuard is a fully open-source tool to scan the NuGet dependencies of your .NET solutions against a deny- or allowlist…",
                    Readme = "See PackageGuard page on NuGet.",
                    LicenseUrl = "", // Not specified
                    License = "", // Not specified
                    ProjectUrl = "", // Not specified
                    IconUrl = "",
                    RepositoryUrl = "",
                    Owner = ""
                },
                // If an earlier version is desired – e.g., previous stable
                new VersionInfo
                {
                    Version = "1.4.0", // Hypothetical prior release
                    Description = "Previous release of PackageGuard",
                    Readme = "",
                    LicenseUrl = "",
                    License = "",
                    ProjectUrl = "",
                    IconUrl = "",
                    RepositoryUrl = "",
                    Owner = ""
                }
            }
        },
        new()
        {
            Id = "Pathy",
            TotalDownloads = 1449,
            Versions = new[]
            {
                new VersionInfo
                {
                    Version = "0.2.2", // This corresponds to “Path” which may differ from “Pathy”
                    Description = "Path is a command-line tool to manage PATH environment variable on Windows...",
                    Readme = "See Path NuGet page.",
                    LicenseUrl = "",
                    License = "",
                    ProjectUrl = "",
                    IconUrl = "",
                    RepositoryUrl = "",
                    Owner = ""
                },
                new VersionInfo
                {
                    Version = "0.2.1", // Hypothetical earlier version
                    Description = "Previous Path release",
                    Readme = "",
                    LicenseUrl = "",
                    License = "",
                    ProjectUrl = "",
                    IconUrl = "",
                    RepositoryUrl = "",
                    Owner = ""
                }
            }
        },
        new()
        {
            Id = "DotNetLibraryPackageTemplates",
            TotalDownloads = 1300,
            Versions = new[]
            {
                new VersionInfo
                {
                    Version = "1.4.3", // Most recent
                    Description =
                        "A dotnet new template for a .NET class library package with all the necessary components to publish it on NuGet.",
                    Readme = "See DotNetLibraryPackageTemplates NuGet page.",
                    LicenseUrl = "",
                    License = "",
                    ProjectUrl = "",
                    IconUrl = "",
                    RepositoryUrl = "",
                    Owner = ""
                },
                new VersionInfo
                {
                    Version = "1.4.2", // Prior version
                    Description = "Previous version of the template package",
                    Readme = "",
                    LicenseUrl = "",
                    License = "",
                    ProjectUrl = "",
                    IconUrl = "",
                    RepositoryUrl = "",
                    Owner = ""
                }
            }
        }
    ];
}
