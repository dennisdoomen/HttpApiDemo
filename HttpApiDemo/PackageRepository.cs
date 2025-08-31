using System.Collections.Concurrent;
using System.Text.Json;

namespace HttpApiDemo;

public class PackageRepository : IRequirePackageInformation
{
    private readonly ConcurrentDictionary<string, PackageInfo> packages =
        new(StringComparer.CurrentCultureIgnoreCase);

    public PackageRepository()
    {
        // Seed initial data
        foreach (var p in examplePackages)
        {
            packages[p.Id] = p;
        }
    }

    public Task<PackageInfo?> FindPackageInfo(string packageId)
    {
        packages.TryGetValue(packageId, out var package);
        return Task.FromResult(package?.IsPending is true ? null : package);
    }

    public Task<IEnumerable<(string Id, string Description)>> GetPackageList() => Task.FromResult(
        packages.Values
            .Where(x => !x.IsPending)
            .Select(x => (x.Id, x.Versions.FirstOrDefault()?.Description ?? string.Empty))
            .AsEnumerable());

    public Task<bool> UpsertPackage(string packageId, int totalDownloads, IEnumerable<VersionInfo> versions)
    {
        var created = !packages.ContainsKey(packageId);
        var normalized = new PackageInfo
        {
            Id = packageId,
            TotalDownloads = totalDownloads,
            Versions = versions?.ToArray() ?? Array.Empty<VersionInfo>()
        };

        packages.AddOrUpdate(packageId, _ => normalized, (_, _) => normalized);
        return Task.FromResult(created);
    }

    public Task<bool> PatchPackage(string packageId, int? totalDownloads, IEnumerable<VersionInfo>? versions)
    {
        if (!packages.TryGetValue(packageId, out var existing))
        {
            return Task.FromResult(false);
        }

        var updated = new PackageInfo
        {
            Id = existing.Id,
            TotalDownloads = totalDownloads ?? existing.TotalDownloads,
            Versions = versions?.ToArray() ?? existing.Versions
        };

        packages[packageId] = updated;
        return Task.FromResult(true);
    }

    public Task<bool> DeletePackage(string packageId)
    {
        return Task.FromResult(packages.TryRemove(packageId, out _));
    }

    public Task<string> UploadPackage(string body)
    {
        // We pretend that the body is being uploaded to a package repository and that this takes
        // a while. For this demo, we parse the complete package data.
        var newPackage = JsonSerializer.Deserialize<NewPackageWithVersions>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (newPackage is null)
        {
            throw new ArgumentException("The body does not contain a valid package");
        }

        string pendingId = (packages.Count + 1).ToString();

        // Convert versions to VersionInfo format
        var versions = newPackage.Versions?.Select(v => new VersionInfo
        {
            Version = v.Version ?? "",
            Description = v.Description ?? "",
            RepositoryUrl = v.RepositoryUrl ?? "",
            Owner = v.Owner ?? "",
            Readme = "",
            LicenseUrl = "",
            License = "",
            ProjectUrl = "",
            IconUrl = ""
        }).ToArray() ?? Array.Empty<VersionInfo>();

        packages.TryAdd(newPackage.Id, new PackageInfo
        {
            Id = newPackage.Id,
            IsPending = true,
            PendingId = pendingId,
            Versions = versions
        });

        return Task.FromResult(pendingId);
    }

    public Task<(UploadStatus Completed, string? Id)> GetUploadStatus(string pendingId)
    {
        PackageInfo? package = packages.Values.FirstOrDefault(p => p.PendingId == pendingId);
        if (package is null)
        {
            return Task.FromResult<(UploadStatus, string?)>((UploadStatus.NotFound, null));
        }

        if (package.IsPending)
        {
            // Pretend that the processing of the package has completed after the first time
            // somebody requests a status update.
            package.IsPending = false;

            return Task.FromResult<(UploadStatus, string?)>((UploadStatus.InProgress, null));
        }
        else
        {
            package.PendingId = null;
            return Task.FromResult<(UploadStatus, string?)>((UploadStatus.Completed, package.Id));
        }
    }

    // Initial seed data (unchanged)
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
            TotalDownloads = 1843,
            Versions = new[]
            {
                new VersionInfo
                {
                    Version = "1.5.0", // This corresponds to “Path” which may differ from “Pathy”
                    Description = "Fluently building and using file and directory paths without binary dependencies",
                    Readme = "Fluently building and using file and directory paths without binary dependencies",
                    LicenseUrl = "https://github.com/dennisdoomen/pathy/blob/main/LICENSE",
                    License = "MIT",
                    ProjectUrl = "https://github.com/dennisdoomen/pathy",
                    IconUrl = "",
                    RepositoryUrl = "https://github.com/dennisdoomen/pathy",
                    Owner = "dennisdoomen"
                },
                new VersionInfo
                {
                    Version = "1.0.0", // Hypothetical earlier version
                    Description = "Fluently building and using file and directory paths without binary dependencies",
                    Readme = "Fluently building and using file and directory paths without binary dependencies",
                    LicenseUrl = "https://github.com/dennisdoomen/pathy/blob/main/LICENSE",
                    License = "MIT",
                    ProjectUrl = "https://github.com/dennisdoomen/pathy",
                    IconUrl = "",
                    RepositoryUrl = "https://github.com/dennisdoomen/pathy",
                    Owner = "dennisdoomen"
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

    private record NewPackageWithVersions(string Id, VersionSummary[]? Versions);

    private record VersionSummary(string? Version, string? Description, string? RepositoryUrl, string? Owner);
}
