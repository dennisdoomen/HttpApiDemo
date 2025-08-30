namespace HttpApiDemo;

/// <summary>
/// Represents a contract for retrieving and managing information about software packages.
/// </summary>
/// <remarks>
/// Notice that this interface is owned by the service layer.
/// </remarks>
public interface IRequirePackageInformation
{
    /// <summary>
    /// Retrieves information about a software package based on its unique identifier.
    /// </summary>
    /// <param name="packageId">The unique identifier of the package to retrieve information for.</param>
    /// <returns>A <see cref="PackageInfo"/> instance containing details about the package, or null if the package is not found.</returns>
    Task<PackageInfo?> FindPackageInfo(string packageId);

    /// <summary>
    /// Retrieves a list of packages, including their unique identifiers and descriptions.
    /// </summary>
    /// <returns>A collection of tuples where each tuple contains the package ID and its description.</returns>
    Task<IEnumerable<(string Id, string Description)>> GetPackageList();

    /// <summary>
    /// Creates a new package registration or replaces an existing registration (idempotent).
    /// </summary>
    /// <param name="packageId">The package identifier to create or replace.</param>
    /// <param name="totalDownloads">Total downloads counter for the registration.</param>
    /// <param name="versions">The versions to associate with this registration.</param>
    /// <returns><c>true</c> if a new registration was created; <c>false</c> if an existing registration was replaced.</returns>
    Task<bool> UpsertPackage(string packageId, int totalDownloads, IEnumerable<VersionInfo> versions);

    /// <summary>
    /// Applies a partial update to an existing package (idempotent by replacement of provided fields).
    /// </summary>
    /// <param name="packageId">The package to update.</param>
    /// <param name="totalDownloads">Optional new total downloads value.</param>
    /// <param name="versions">Optional replacement for the versions collection. When provided, replaces the complete set.</param>
    /// <returns>The updated <see cref="PackageInfo"/> or <c>null</c> if the package does not exist.</returns>
    Task<bool> PatchPackage(string packageId, int? totalDownloads, IEnumerable<VersionInfo>? versions);

    /// <summary>
    /// Deletes the package registration (idempotent).
    /// </summary>
    /// <param name="packageId">The package to delete.</param>
    /// <returns><c>true</c> if it existed and was deleted; otherwise <c>false</c>.</returns>
    Task<bool> DeletePackage(string packageId);
}
