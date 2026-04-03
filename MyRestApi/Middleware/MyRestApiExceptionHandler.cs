using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MyRestApi.Middleware;

public class MyRestApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            UnauthorizedException =>
                (StatusCodes.Status401Unauthorized, "Unauthorized."),

            ConflictException =>
                (StatusCodes.Status409Conflict, "Conflict."),

            NotFoundException =>
                (StatusCodes.Status404NotFound, "Not found."),

            GraphQLServiceException { InnerException: System.Net.Sockets.SocketException } =>
                (StatusCodes.Status503ServiceUnavailable, "GraphQL service is unreachable."),

            GraphQLServiceException =>
                (StatusCodes.Status502BadGateway, "GraphQL service returned an error."),

            RestApiServiceException { InnerException: System.Net.Sockets.SocketException } =>
                (StatusCodes.Status503ServiceUnavailable, "REST API service is unreachable."),

            RestApiServiceException =>
                (StatusCodes.Status502BadGateway, "REST API service returned an error."),

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
