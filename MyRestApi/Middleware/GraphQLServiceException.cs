namespace MyRestApi.Middleware;

public class GraphQLServiceException : Exception
{
    public GraphQLServiceException(string message, Exception inner) : base(message, inner) { }
}