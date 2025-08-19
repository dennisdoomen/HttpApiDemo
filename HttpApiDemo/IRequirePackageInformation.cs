namespace HttpApiDemo;

/// <summary>
/// Represents a contract for retrieving information about software packages.
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

    Task<IEnumerable<(string Id, string Description)>> GetPackageList();
}
