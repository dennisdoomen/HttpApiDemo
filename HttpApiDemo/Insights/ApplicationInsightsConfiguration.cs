namespace HttpApiDemo.Insights;

internal static class ApplicationInsightsConfiguration
{
    /// <summary>
    /// Configures the API to use Application Insights, configured for Flux.
    /// </summary>
    internal static WebApplicationBuilder AddAppInsights(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        var connectionString = configuration["ApplicationInsights:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ApplicationInsights:ConnectionString is not set or empty");
        }

        builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = connectionString;
        });

        // Set minimum log level for Application Insights logs (in 3.x, logging is handled via OpenTelemetry)
        builder.Logging.AddFilter("", LogLevel.Information);

        return builder;
    }
}
