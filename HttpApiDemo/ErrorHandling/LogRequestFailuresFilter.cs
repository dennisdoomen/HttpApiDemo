using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HttpApiDemo.ErrorHandling;

/// <summary>
/// Automatically logs all responses with a status code of 400 or higher as a warning.
/// </summary>
/// <remarks>
/// This avoids the need for unnecessary logs anytime a controller action returns non-success response.
/// </remarks>
internal class LogRequestFailuresFilter(ILogger<LogRequestFailuresFilter> logger) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ArgumentException)
        {
            context.Result = new BadRequestResult();
            return;
        }

        if (context.Exception is not null)
        {
            logger.LogError(context.Exception, "Request {Url} failed with exception",
                context.HttpContext.Request.GetDisplayUrl());

            return;
        }

        switch (context.Result)
        {
            case ObjectResult { StatusCode: >= 400 } objectResult:
            {
                logger.LogWarning("Request {Url} failed with status {StatusCode}: {Message}",
                    context.HttpContext.Request.GetDisplayUrl(),
                    objectResult.StatusCode,
                    objectResult.Value!.ToString());

                break;
            }

            case StatusCodeResult { StatusCode: >= 400 } statusCodeResult:
            {
                logger.LogWarning("Request {Url} failed with status {StatusCode}",
                    context.HttpContext.Request.GetDisplayUrl(),
                    statusCodeResult.StatusCode);

                break;
            }

            default:
            {
                // We don't want to log successful requests
                break;
            }
        }
    }
}
