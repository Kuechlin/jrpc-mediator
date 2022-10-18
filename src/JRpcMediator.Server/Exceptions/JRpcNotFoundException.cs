namespace JRpcMediator.Server.Exceptions;

public class JRpcNotFoundException : Exception
{
    public JRpcNotFoundException()
    {
    }

    public JRpcNotFoundException(string? message) : base(message)
    {
    }

    public JRpcNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}