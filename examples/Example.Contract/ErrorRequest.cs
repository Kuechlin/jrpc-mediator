using JRpcMediator;
using MediatR;

namespace Example.Contract;

[JRpcMethod("error")]
public class ErrorRequest : IRequest<string>
{
    public ErrorRequest(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}
