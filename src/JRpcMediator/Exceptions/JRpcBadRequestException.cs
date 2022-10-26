namespace JRpcMediator.Exceptions;

public class JRpcBadRequestException : Exception
{
    public JRpcBadRequestException()
    {
    }

    public JRpcBadRequestException(string? message) : base(message)
    {
    }

    public JRpcBadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
