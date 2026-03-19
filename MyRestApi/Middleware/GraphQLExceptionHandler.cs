using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MyRestApi.Middleware;

public class GraphQLExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            HttpRequestException { InnerException: System.Net.Sockets.SocketException } =>
                (StatusCodes.Status503ServiceUnavailable, "GraphQL service is unreachable."),

            HttpRequestException =>
                (StatusCodes.Status502BadGateway, "GraphQL service returned an error."),

            _ =>
                (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = status;

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title  = title,
            Detail = exception.Message
        }, cancellationToken);

        return true;
    }
}
