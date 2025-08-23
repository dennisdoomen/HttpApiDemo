using System.Reflection;
using System.Text.Json.Serialization;
using Asp.Versioning;
using HttpApiDemo.ErrorHandling;
using HttpApiDemo.HealthChecking;
using HttpApiDemo.Infrastructure;
using HttpApiDemo.Insights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

// Creating a separate configuration config is not required. WebApplication.CreateBuilder(args) already creates a configuration for you.
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0#default-application-configuration-sources
namespace HttpApiDemo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Enable support for using .NET User Secrets connected to the current assembly
        // See https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=windows or
        // https://blog.jetbrains.com/dotnet/2023/01/17/securing-sensitive-information-with-net-user-secrets/
        builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

        // Enable a /health endpoint to check the health of the application.
        builder.Services.AddHealthChecking(builder.Configuration);

        // Debugging with app insights is not required.
        if (!builder.Environment.IsDevelopment() && builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString") != null)
        {
            builder.AddAppInsights(builder.Configuration);
        }

        // Allow other domains to access this endpoint as well.
        builder.AddCorsPolicy(builder.Configuration);


        builder.Services.AddControllers(options =>
        {
            // Enable automatic logging of 400 errors.
            options.Filters.Add<LogRequestFailuresFilter>();

        }).ConfigureApiBehaviorOptions(options =>
        {
            var builtInFactory = options.InvalidModelStateResponseFactory;

            options.InvalidModelStateResponseFactory = LoggedInvalidModelStateResponseFactory.Create(builtInFactory);
        });

        // Force all URLs to be lowercase.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // Enable OpenAPI endspoints
        builder.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();

        // Make sure enums are serialized to their name and not their number
        builder.Services.Configure<JsonOptions>(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddSingleton<IRequirePackageInformation, PackageRepository>();

        var app = builder.Build();

        app.UseSwaggerUi();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();
        app.MapHealthChecks("/health");
        app.MapControllers();

        // Add Minimal API endpoints alongside controllers
        MapMinimalApiEndpoints(app);

        app.Run();
    }

    private static void MapMinimalApiEndpoints(WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .HasApiVersion(new ApiVersion(2, 0))
            .ReportApiVersions()
            .Build();

        // GET /minimal-api/packages/ (v2.0) - Returns package list
        app.MapGet("/minimal-api/v{version:apiVersion}/packages", GetPackagesMinimalAsync)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(new ApiVersion(2, 0))
            .WithName("GetPackagesMinimal")
            .WithSummary("Retrieves a list of packages available for download using Minimal API")
            .WithOpenApi()
            .Produces<object[]>(StatusCodes.Status200OK);

        // GET /minimal-api/packages/{id} (v1.0) - Returns package details (limited info)
        app.MapGet("/minimal-api/v{version:apiVersion}/packages/{packageId}", GetPackageByIdV1MinimalAsync)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(new ApiVersion(1, 0))
            .WithName("GetPackageByIdV1Minimal")
            .WithSummary("Retrieves package information with limited details using Minimal API (v1.0)")
            .WithOpenApi()
            .Produces<object>(StatusCodes.Status200OK)
            .Produces<object>(StatusCodes.Status404NotFound)
            .Produces<object>(StatusCodes.Status400BadRequest);

        // GET /minimal-api/packages/{id} (v2.0) - Returns package details (full info)
        app.MapGet("/minimal-api/v{version:apiVersion}/packages/{packageId}", GetPackageByIdV2MinimalAsync)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(new ApiVersion(2, 0))
            .WithName("GetPackageByIdV2Minimal")
            .WithSummary("Retrieves package information with full details using Minimal API (v2.0)")
            .WithOpenApi()
            .Produces<object>(StatusCodes.Status200OK)
            .Produces<object>(StatusCodes.Status404NotFound)
            .Produces<object>(StatusCodes.Status400BadRequest);

        // GET /minimal-api/packages/{id}/statistics (v2.0) - Returns package statistics
        app.MapGet("/minimal-api/v{version:apiVersion}/packages/{packageId}/statistics", GetStatisticsMinimalAsync)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(new ApiVersion(2, 0))
            .WithName("GetStatisticsMinimal")
            .WithSummary("Retrieves package statistics using Minimal API")
            .WithOpenApi()
            .Produces<object>(StatusCodes.Status200OK)
            .Produces<object>(StatusCodes.Status404NotFound)
            .Produces<object>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetPackagesMinimalAsync(IRequirePackageInformation packageProvider, HttpContext context)
    {
        // Set cache headers same as controller
        context.Response.Headers.Expires = DateTime.UtcNow.Date.Add(new TimeSpan(0, 59, 00)).ToString("R");
        context.Response.Headers.CacheControl = new CacheControlHeaderValue
        {
            Private = false,
            Public = true,
        }.ToString();

        var packages = await packageProvider.GetPackageList();

        return Results.Ok(packages.Select(x => new
        {
            Id = x.Id,
            Description = x.Description,
        }));
    }

    private static async Task<IResult> GetPackageByIdV1MinimalAsync(
        string packageId, 
        IRequirePackageInformation packageProvider, 
        HttpContext context,
        ILogger<Program> logger)
    {
        // Set cache headers same as controller
        context.Response.Headers.Expires = DateTime.UtcNow.Date.Add(new TimeSpan(0, 59, 00)).ToString("R");
        context.Response.Headers.CacheControl = new CacheControlHeaderValue
        {
            Private = false,
            Public = true,
        }.ToString();

        if (string.IsNullOrWhiteSpace(packageId))
        {
            logger.LogWarning("A non-empty package ID is required");
            return Results.BadRequest(new
            {
                Error = "A non-empty package ID is required"
            });
        }

        PackageInfo? package = await packageProvider.FindPackageInfo(packageId);
        if (package is null)
        {
            logger.LogWarning("Could not find a package with ID {packageId}", packageId);
            return Results.NotFound(new
            {
                Error = $"Could not find a package with ID {packageId}"
            });
        }

        return Results.Ok(new
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

    private static async Task<IResult> GetPackageByIdV2MinimalAsync(
        string packageId, 
        IRequirePackageInformation packageProvider, 
        HttpContext context,
        ILogger<Program> logger)
    {
        // Set cache headers same as controller
        context.Response.Headers.Expires = DateTime.UtcNow.Date.Add(new TimeSpan(0, 59, 00)).ToString("R");
        context.Response.Headers.CacheControl = new CacheControlHeaderValue
        {
            Private = false,
            Public = true,
        }.ToString();

        if (string.IsNullOrWhiteSpace(packageId))
        {
            logger.LogWarning("A non-empty package ID is required");
            return Results.BadRequest(new
            {
                Error = "A non-empty package ID is required"
            });
        }

        PackageInfo? package = await packageProvider.FindPackageInfo(packageId);
        if (package is null)
        {
            logger.LogWarning("Could not find a package with ID {packageId}", packageId);
            return Results.NotFound(new
            {
                Error = $"Could not find a package with ID {packageId}"
            });
        }

        return Results.Ok(new
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

    private static async Task<IResult> GetStatisticsMinimalAsync(
        string packageId, 
        IRequirePackageInformation packageProvider, 
        HttpContext context,
        ILogger<Program> logger)
    {
        // Set cache headers same as controller
        context.Response.Headers.Expires = DateTime.UtcNow.Date.Add(new TimeSpan(0, 59, 00)).ToString("R");
        context.Response.Headers.CacheControl = new CacheControlHeaderValue
        {
            Private = false,
            Public = true,
        }.ToString();

        if (string.IsNullOrWhiteSpace(packageId))
        {
            logger.LogWarning("A non-empty package ID is required");
            return Results.BadRequest(new
            {
                Error = "A non-empty package ID is required"
            });
        }

        PackageInfo? package = await packageProvider.FindPackageInfo(packageId);
        if (package is null)
        {
            logger.LogWarning("Could not find a package with ID {packageId}", packageId);
            return Results.NotFound(new
            {
                Error = $"Could not find a package with ID {packageId}"
            });
        }

        return Results.Ok(new
        {
            Id = package.Id,
            TotalDownloads = package.TotalDownloads
        });
    }
}
