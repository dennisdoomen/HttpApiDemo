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

    /// <summary>
    /// Retrieves a list of packages, including their unique identifiers and descriptions.
    /// </summary>
    /// <returns>A collection of tuples where each tuple contains the package ID and its description.</returns>
    Task<IEnumerable<(string Id, string Description)>> GetPackageList();
}
