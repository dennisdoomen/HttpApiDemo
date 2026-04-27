using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HttpApiDemo.Telemetry;

internal static class OpenTelemetryConfiguration
{
    private const string ServiceName = "HttpApiDemo";

    /// <summary>
    /// Configures OpenTelemetry tracing, metrics, and logging.
    /// In development, signals are written to the console.
    /// In all environments, signals are exported via OTLP when
    /// <c>OpenTelemetry:Endpoint</c> is configured.
    /// </summary>
    internal static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var resource = ResourceBuilder.CreateDefault()
            .AddService(ServiceName)
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector();

        string? otlpEndpoint = builder.Configuration["OpenTelemetry:Endpoint"];

        builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resource)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        // Don't trace health check and OpenAPI/Scalar endpoints
                        options.Filter = ctx =>
                            !ctx.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase) &&
                            !ctx.Request.Path.StartsWithSegments("/scalar", StringComparison.OrdinalIgnoreCase) &&
                            !ctx.Request.Path.StartsWithSegments("/api-docs", StringComparison.OrdinalIgnoreCase);
                    })
                    .AddHttpClientInstrumentation();

                if (builder.Environment.IsDevelopment())
                {
                    tracing.AddConsoleExporter();
                }

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resource)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (builder.Environment.IsDevelopment())
                {
                    metrics.AddConsoleExporter();
                }

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                {
                    metrics.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
                }
            });

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.SetResourceBuilder(resource);
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            if (builder.Environment.IsDevelopment())
            {
                logging.AddConsoleExporter();
            }

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                logging.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint);
                    options.Protocol = OtlpExportProtocol.Grpc;
                });
            }
        });

        return builder;
    }
}
