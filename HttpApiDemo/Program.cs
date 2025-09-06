using System.Reflection;
using System.Text.Json.Serialization;
using HttpApiDemo.HealthChecking;
using HttpApiDemo.Infrastructure;
using HttpApiDemo.Insights;
using Microsoft.AspNetCore.Mvc;

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

        // Add support for returning problem details for failing actions (for demo purposes as this is the default)
        builder.Services.AddProblemDetails();

        // Debugging with app insights is not required.
        if (!builder.Environment.IsDevelopment() &&
            !string.IsNullOrWhiteSpace(builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString")))
        {
            builder.AddAppInsights(builder.Configuration);
        }

        // Allow other domains to access this endpoint as well.
        builder.AddCorsPolicy(builder.Configuration);

        builder.Services.AddControllers();

        // Force all URLs to be lowercase.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // Enable OpenAPI endspoints
        builder.AddOpenApi();

        // Make sure enums are serialized to their name and not their number
        builder.Services.Configure<JsonOptions>(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddSingleton<IRequirePackageInformation, PackageRepository>();

        var app = builder.Build();

        // Make sure unhandled exceptions are also converted to problem details.
        app.UseExceptionHandler();

        app.UseOpenApiUi();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();
        app.MapHealthChecks("/health");
        app.MapControllers();

        app.Run();
    }
}
