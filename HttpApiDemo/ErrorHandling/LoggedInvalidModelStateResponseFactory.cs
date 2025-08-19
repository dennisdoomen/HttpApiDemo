using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HttpApiDemo.ErrorHandling;

/// <summary>
/// Adds automatic logging of the ASP.NET Core model state validation failures.
/// </summary>
internal static class LoggedInvalidModelStateResponseFactory
{
    internal static Func<ActionContext, IActionResult> Create(Func<ActionContext, IActionResult> builtInFactory) =>
        context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            logger.LogWarning("Request {Url} failed with model state: {ModelState}",
                context.HttpContext.Request.GetDisplayUrl(),
                context.ModelState.ValidationState);

            return builtInFactory(context);
        };
}
