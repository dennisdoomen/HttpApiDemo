namespace HttpApiDemo.HealthChecking;

internal static class ServiceExtensions
{
    /// <summary>
    /// Configures and adds health checking features to the service collection using the specified configuration.
    /// </summary>
    /// <param name="services">The service collection to which the health checking feature will be added.</param>
    /// <param name="configuration">The configuration used to retrieve necessary settings for the health checking features.</param>
    /// <returns>The updated service collection with the added health checking feature.</returns>
    public static IServiceCollection AddHealthChecking(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApplicationInsightsHealthCheckOptions>(o => o.ConnectionString = configuration.GetValue<string>("ApplicationInsights:ConnectionString")!);
        services.AddHttpClient(ApplicationInsightsHealthCheck.ApplicationInsightsHttpClientName);

        services.AddHealthChecks()
            .AddCheck<ApplicationInsightsHealthCheck>("ApplicationInsights");

        return services;
    }
}
