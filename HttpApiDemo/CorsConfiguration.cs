namespace HttpApiDemo;

internal static class CorsConfiguration
{
    /// <summary>
    /// Add a CORS policy to make sure that the configured origins have access to the API endpoints.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configuration">The web application configuration.</param>
    /// <returns>WebApplicationBuilder</returns>
    internal static WebApplicationBuilder AddCorsPolicy(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        string? origins = configuration["Cors:Origins"];

        if (!string.IsNullOrWhiteSpace(origins))
        {
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => policy.WithOrigins(origins.Split(','))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });
        }

        return builder;
    }
}
