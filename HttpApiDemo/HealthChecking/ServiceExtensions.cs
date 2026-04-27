namespace HttpApiDemo.HealthChecking;

internal static class ServiceExtensions
{
    /// <summary>
    /// Configures and adds health checking features to the service collection.
    /// </summary>
    public static IServiceCollection AddHealthChecking(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}
