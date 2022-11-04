using JRpcMediator;
using MediatR;

namespace Example.Contract;

[JRpcMethod("error")]
public class ErrorRequest : IRequest<Result<string>>
{
    public ErrorRequest(bool shouldThrow, string message)
    {
        ShouldThrow = shouldThrow;
        Message = message;
    }

    public bool ShouldThrow { get; set; }
    public string Message { get; set; }
}
