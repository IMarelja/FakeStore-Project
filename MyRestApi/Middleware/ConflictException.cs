namespace MyRestApi.Middleware;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
