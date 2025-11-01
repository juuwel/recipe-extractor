using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecipeService.Domain.Exceptions;

namespace RecipeService.Api.FIlters;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Default values
        var statusCode = StatusCodes.Status500InternalServerError;
        var title = "Server error";
        var detail = exception.Message;
        var additionalData = new Dictionary<string, object>();

        if (exception is CustomException customException)
        {
            logger.LogError(exception, "Custom exception occurred: {Message}", exception.Message);

            switch (exception)
            {
                // case NotFoundException notFoundException:
                //     statusCode = StatusCodes.Status404NotFound;
                //     title = "NotFound";
                //     detail = notFoundException.Message;
                //     break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    title = "CustomExceptionNotImplemented";
                    detail = customException.Message;
                    break;
            }
        }
        else
        {
            logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Extensions = additionalData!
        };
        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
