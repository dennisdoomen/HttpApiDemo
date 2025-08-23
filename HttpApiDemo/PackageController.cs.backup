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
    /// <summary>
    /// Retrieves a list of packages available for download.
    /// </summary>
    [HttpGet]
    [Route("")]
    [MapToApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPackages()
    {
        Response.Headers.Expires = DateTime.UtcNow.Date.Add(new TimeSpan(0, 59, 00)).ToString("R");
        Response.Headers.CacheControl = new CacheControlHeaderValue
        {
            Private = false,
            Public = true,
        }.ToString();

        var packages = await packageProvider.GetPackageList();

        return Ok(packages.Select(x => new
        {
            Id = x.Id,
            Description = x.Description,
        }));
    }

    /// <summary>
    /// Retrieves information and statistics about a specific package.
    /// </summary>
    /// <param name="packageId">The unique identifier of the package.</param>
    /// <returns>An instance of <see cref="PackageInfo"/> containing the package statistics if found, or an appropriate status response if the package is not found or the input is invalid.</returns>
    [HttpGet]
    [Route("{packageId}")]
    [MapToApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "internal")]
    [ProducesResponseType(typeof(PackageInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetPackageByIdV1(string packageId)
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
            return BadRequest(new
            {
                Error = "A non-empty package ID is required"
            });
        }

        PackageInfo? package = await packageProvider.FindPackageInfo(packageId);
        if (package is null)
        {
            logger.LogWarning("Could not find a package with ID {packageId}", packageId);
            return NotFound(new
            {
                Error = $"Could not find a package with ID {packageId}"
            });
        }

        return Ok(new
        {
            Id = package.Id,
            Versions = package.Versions.Select(v => new
            {
                v.Version,
                v.Description,
                v.RepositoryUrl,
                v.Owner
            })
        });
    }

    /// <summary>
    /// Retrieves information and statistics about a specific package.
    /// </summary>
    /// <param name="packageId">The unique identifier of the package.</param>
    /// <returns>An instance of <see cref="PackageInfo"/> containing the package statistics if found, or an appropriate status response if the package is not found or the input is invalid.</returns>
    [HttpGet]
    [Route("{packageId}")]
    [MapToApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "internal")]
    [ProducesResponseType(typeof(PackageInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetPackageByIdV2(string packageId)
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
            return BadRequest(new
            {
                Error = "A non-empty package ID is required"
            });
        }

        PackageInfo? package = await packageProvider.FindPackageInfo(packageId);
        if (package is null)
        {
            logger.LogWarning("Could not find a package with ID {packageId}", packageId);
            return NotFound(new
            {
                Error = $"Could not find a package with ID {packageId}"
            });
        }

        return Ok(new
        {
            Id = package.Id,
            Versions = package.Versions.Select(v => new
            {
                v.Version,
                v.Description,
                v.Readme,
                v.LicenseUrl,
                v.ProjectUrl,
                v.IconUrl,
                v.RepositoryUrl,
                v.Owner
            })
        });
    }

    /// <summary>
    /// Retrieves information and statistics about a specific package.
    /// </summary>
    /// <param name="packageId">The unique identifier of the package.</param>
    /// <returns>An instance of <see cref="PackageInfo"/> containing the package statistics if found, or an appropriate status response if the package is not found or the input is invalid.</returns>
    [HttpGet]
    [Route("{packageId}/statistics")]
    [ApiExplorerSettings(GroupName = "private")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(PackageInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetStatistics(string packageId)
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
            return BadRequest(new
            {
                Error = "A non-empty package ID is required"
            });
        }

        PackageInfo? package = await packageProvider.FindPackageInfo(packageId);
        if (package is null)
        {
            logger.LogWarning("Could not find a package with ID {packageId}", packageId);
            return NotFound(new
            {
                Error = $"Could not find a package with ID {packageId}"
            });
        }

        return Ok(new
        {
            Id = package.Id,
            TotalDownloads = package.TotalDownloads
        });
    }

    [HttpGet]
    [Route("")]
    [MapToApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "public")]
    public Task<IActionResult> GetPackages([FromQuery] string packageId)
    {
        return Task.FromResult<IActionResult>(NotFound("This endpoint is deprecated."));
    }
}
