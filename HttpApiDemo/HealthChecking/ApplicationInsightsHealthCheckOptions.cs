namespace HttpApiDemo.HealthChecking;

/// <summary>
/// Provides configuration options for the ApplicationInsightsHealthCheck.
/// This class is used to specify the connection string required to validate the health of Application Insights.
/// </summary>
internal class ApplicationInsightsHealthCheckOptions
{
    /// <summary>
    /// Gets or sets the connection string for Application Insights.
    /// </summary>
    /// <remarks>
    /// This connection string is used to configure and verify connectivity
    /// to the Application Insights service. It must include both the
    /// InstrumentationKey and IngestionEndpoint values in the expected format.
    /// </remarks>
    public required string ConnectionString { get; set; }
}
