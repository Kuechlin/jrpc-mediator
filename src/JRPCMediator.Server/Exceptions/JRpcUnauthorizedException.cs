using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JRpcMediator.Server.Exceptions;

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
