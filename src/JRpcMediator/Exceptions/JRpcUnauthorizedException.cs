using System;

namespace JRpcMediator.Exceptions
{
    public class JRpcUnauthorizedException : Exception
    {
        public JRpcUnauthorizedException()
        {
        }

        public JRpcUnauthorizedException(string? message) : base(message)
        {
        }

        public JRpcUnauthorizedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}