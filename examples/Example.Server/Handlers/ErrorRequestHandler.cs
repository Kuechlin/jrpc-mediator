using Example.Contract;
using JRpcMediator;
using JRpcMediator.Exceptions;
using MediatR;

namespace Example.Server.Handlers;

public class ErrorRequestHandler : IRequestHandler<ErrorRequest, Result<string>>
{
    public Task<Result<string>> Handle(ErrorRequest request, CancellationToken cancellationToken)
    {
        var ex = new Exception(request.Message);

        if (request.ShouldThrow)
        {
            throw ex;
        }
        else 
        {
            return Task.FromResult(new Result<string>(ex));
        }
    }
}
