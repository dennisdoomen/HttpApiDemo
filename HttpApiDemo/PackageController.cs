using System.ComponentModel;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace HttpApiDemo;

[ApiController]
[Route("api/v{version:apiVersion}/packages/")]
[Route("api/packages/")]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Produces("application/json")]
public class PackageController(IRequirePackageInformation packageProvider, ILogger<PackageController> logger) : ControllerBase
{
    [EndpointSummary("Retrieves a list of packages available for download")]
    [EndpointDescription("Gets a list of all available packages that can be downloaded")]
    [HttpGet]
    [Route("")]
    [MapToApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "public")]
    [ProducesResponseType(typeof(IEnumerable<PackageResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPackages(
        [FromQuery(Name = "$skip")] int skip = 0,
        [FromQuery(Name = "$take")] int take = 100)
    {
        Response.Headers.Expires = DateTime.UtcNow.Date.Add(new TimeSpan(0, 59, 00)).ToString("R");
        Response.Headers.CacheControl = new CacheControlHeaderValue
        {
            Private = false,
            Public = true,
        }.ToString();

        var packages = await packageProvider.GetPackageList();

        IEnumerable<PackageResponse> response = packages
            .Skip(skip)
            .Take(take)
            .Select(x => new PackageResponse
        {
            Id = x.Id,
            Description = x.Description,
        });

        return Ok(response);
    }

    [EndpointSummary("Retrieves information and statistics about a specific package")]
    [EndpointDescription("Gets detailed information about a specific package by its unique identifier. Returns an instance of PackageInfo containing the package statistics if found, or an appropriate status response if the package is not found or the input is invalid.")]
    [HttpGet]
    [Route("{packageId}")]
    [MapToApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "internal")]
    [ProducesResponseType(typeof(PackageWithVersionSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPackageByIdV1([Description("The unique identifier of the package")] string packageId)
    {
        Response.Headers.Expires = DateTime.UtcNow.Date.Add(new TimeSpan(0, 59, 00)).ToString("R");
        Response.Headers.CacheControl = new CacheControlHeaderValue
        {
            Private = false,
            Public = true,
        }.ToString();

        if (string.IsNullOrWhiteSpace(packageId))
        {
            logger.LogWarning("A non-empty package ID is required");
            return BadRequest(new ErrorResponse { Error = "A non-empty package ID is required" });
        }

        PackageInfo? package = await packageProvider.FindPackageInfo(packageId);
        if (package is null)
        {
            logger.LogWarning("Could not find a package with ID {packageId}", packageId);
            return NotFound(new ErrorResponse { Error = $"Could not find a package with ID {packageId}" });
        }

        var response = new PackageWithVersionSummaryResponse
        {
            Id = package.Id,
            Versions = package.Versions.Select(v => new VersionSummary
            {
                Version = v.Version,
                Description = v.Description,
                RepositoryUrl = v.RepositoryUrl,
                Owner = v.Owner
            }).ToList()
        };
        return Ok(response);
    }

    [EndpointSummary("Retrieves information and statistics about a specific package")]
    [EndpointDescription("Gets detailed information about a specific package by its unique identifier (v2.0 includes additional metadata like readme, license, and project URLs). Returns an instance of PackageInfo containing the package statistics if found, or an appropriate status response if the package is not found or the input is invalid.")]
    [HttpGet]
    [Route("{packageId}")]
    [MapToApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "internal")]
    [ProducesResponseType(typeof(PackageWithVersionDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPackageByIdV2([Description("The unique identifier of the package")] string packageId)
    {
        Response.Headers.Expires = DateTime.UtcNow.Date.Add(new TimeSpan(0, 59, 00)).ToString("R");
        Response.Headers.CacheControl = new CacheControlHeaderValue
        {
            Private = false,
            Public = true,
        }.ToString();

        if (string.IsNullOrWhiteSpace(packageId))
        {
            logger.LogWarning("A non-empty package ID is required");
            return BadRequest(new ErrorResponse { Error = "A non-empty package ID is required" });
        }

        PackageInfo? package = await packageProvider.FindPackageInfo(packageId);
        if (package is null)
        {
            logger.LogWarning("Could not find a package with ID {packageId}", packageId);
            return NotFound(new ErrorResponse { Error = $"Could not find a package with ID {packageId}" });
        }

        var response = new PackageWithVersionDetailsResponse
        {
            Id = package.Id,
            Versions = package.Versions.Select(v => new VersionDetails
            {
                Version = v.Version,
                Description = v.Description,
                Readme = v.Readme,
                LicenseUrl = v.LicenseUrl,
                ProjectUrl = v.ProjectUrl,
                IconUrl = v.IconUrl,
                RepositoryUrl = v.RepositoryUrl,
                Owner = v.Owner
            }).ToList()
        };
        return Ok(response);
    }

    [EndpointSummary("Retrieves statistics about a specific package")]
    [EndpointDescription("Gets download statistics and other metrics for a specific package. Returns an instance of PackageInfo containing the package statistics if found, or an appropriate status response if the package is not found or the input is invalid.")]
    [HttpGet]
    [Route("{packageId}/statistics")]
    [ApiExplorerSettings(GroupName = "private")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(PackageStatistics), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStatistics([Description("The unique identifier of the package")] string packageId)
    {
        Response.Headers.Expires = DateTime.UtcNow.Date.Add(new TimeSpan(0, 59, 00)).ToString("R");
        Response.Headers.CacheControl = new CacheControlHeaderValue
        {
            Private = false,
            Public = false,
            NoCache = true
        }.ToString();

        if (string.IsNullOrWhiteSpace(packageId))
        {
            logger.LogWarning("A non-empty package ID is required");
            return BadRequest(new ErrorResponse { Error = "A non-empty package ID is required" });
        }

        PackageInfo? package = await packageProvider.FindPackageInfo(packageId);
        if (package is null)
        {
            logger.LogWarning("Could not find a package with ID {packageId}", packageId);
            return NotFound(new ErrorResponse { Error = $"Could not find a package with ID {packageId}" });
        }

        var response = new PackageStatistics
        {
            Id = package.Id,
            TotalDownloads = package.TotalDownloads
        };
        return Ok(response);
    }

    [EndpointSummary("Deprecated packages endpoint")]
    [EndpointDescription("This endpoint is deprecated and will return a 404 Not Found response")]
    [HttpGet]
    [Route("")]
    [MapToApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IActionResult> GetPackages(
        [FromQuery]
        [Description("Package identifier parameter (not used in this deprecated endpoint)")]
        string packageId)
    {
        return Task.FromResult<IActionResult>(NotFound("This endpoint is deprecated."));
    }
}
