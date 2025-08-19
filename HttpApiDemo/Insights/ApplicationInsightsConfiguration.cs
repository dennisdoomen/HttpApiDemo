using System.Configuration;
using Microsoft.Extensions.Logging.ApplicationInsights;

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
            throw new ConfigurationErrorsException("ApplicationInsights:ConnectionString is not set or empty");
        }

        builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = connectionString;

            // Normally, telemetry data is sent in batches to minimize the impact on your application's performance. It's collected and
            // stored in a buffer until either the buffer is full or a certain amount of time has passed, then it's sent all together.
            // However, when DeveloperMode is set to true, telemetry data is sent immediately as soon as it's generated, not in batches.
            // This can be useful during development as it means your telemetry data is available for inspection immediately.
            options.DeveloperMode = builder.Environment.IsDevelopment();

            // Also log the telemetry data directly to the debugger's debug output window.
            options.EnableDebugLogger = builder.Environment.IsDevelopment();
        });

        builder.Services.AddLogging(loggingBuilder =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                // Enable logging to the Application Insights service.
                loggingBuilder.AddApplicationInsights(
                    configureTelemetryConfiguration: config => config.ConnectionString = connectionString,
                    configureApplicationInsightsLoggerOptions: options => { }
                );
            }
        });

        // If you set this to "", you will get all the internal tracing from ASP.NET core as well, which is a lot, unless, you set the minimum level to e.g. Information
        // Otherwise use the `FullName` of the T in ILogger<T>
        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information);

        // TODO: Add Oracle connection logging to Application Insights
        return builder;
    }
}
