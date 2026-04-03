namespace MyRestApi.Middleware;

public class RestApiServiceException : Exception
{
    public RestApiServiceException(string message, Exception inner) : base(message, inner) { }
}