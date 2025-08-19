using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace HttpApiDemo.HealthChecking;

/// <summary>
/// ApplicationInsightsHealthCheck checks if it's possible to connect to ApplicationInsights to ensure both the ConnectionString
/// and network setup are correct.
///
/// It checks via an HTTP GET call to {ingestionEndpoint}api/profiles/{instrumentationKey}/appId , both the IngestionPoint
/// and InstrumentationKey are coming from the ConnectionString.
/// </summary>
internal class ApplicationInsightsHealthCheck(
    IOptions<ApplicationInsightsHealthCheckOptions> options,
    ILogger<ApplicationInsightsHealthCheck> logger,
    IHttpClientFactory httpClientFactory) : IHealthCheck
{
    internal const string ApplicationInsightsHttpClientName = "azureappinsights";
    private readonly ApplicationInsightsHealthCheckOptions options = options.Value;

    /// <summary>
    /// Checks if connectivity with the Application Insights is healthy.
    /// </summary>
    /// <returns>Healthy when Application Insights is reachable and returns a valid response.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(options.ConnectionString);

            var connectionStringParts = options.ConnectionString.Split(";");
            var instrumentationKey = connectionStringParts
                .Single(part => part.StartsWith("InstrumentationKey", StringComparison.OrdinalIgnoreCase)).Split("=")[1];

            var ingestionEndpoint = connectionStringParts
                .Single(part => part.StartsWith("IngestionEndpoint", StringComparison.OrdinalIgnoreCase)).Split("=")[1];

            var client = httpClientFactory.CreateClient(ApplicationInsightsHttpClientName);

            var response = await client.GetAsync(new Uri(new Uri(ingestionEndpoint), $"api/profiles/{instrumentationKey}/appId"),
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            return HealthCheckResult.Healthy();
        }
#pragma warning disable AV1210, CA1031 // Explicitly required to capture generic exceptions and return an unhealthy result.
        catch (Exception ex)
#pragma warning restore AV1210
        {
            logger.LogError(ex, "Application Insights health check failed");
            return HealthCheckResult.Degraded(exception: ex);
        }
    }
}
