namespace MyRestApi.Middleware;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
