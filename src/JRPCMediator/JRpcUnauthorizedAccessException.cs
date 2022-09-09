namespace JRpcMediator;

public class JRpcUnauthorizedAccessException : Exception
{
    public JRpcUnauthorizedAccessException()
    {
    }

    public JRpcUnauthorizedAccessException(string? message) : base(message)
    {
    }

    public JRpcUnauthorizedAccessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}