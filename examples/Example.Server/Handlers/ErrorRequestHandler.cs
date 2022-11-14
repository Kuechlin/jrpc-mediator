using Example.Contract;
using JRpcMediator;
using JRpcMediator.Exceptions;
using MediatR;

namespace Example.Server.Handlers;

public class ErrorRequestHandler : IRequestHandler<ErrorRequest, string>
{
    public Task<string> Handle(ErrorRequest request, CancellationToken cancellationToken)
    {
        throw new Exception(request.Message);
    }
}