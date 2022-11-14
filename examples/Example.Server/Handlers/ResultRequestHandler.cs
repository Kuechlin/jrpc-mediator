using Example.Contract;
using JRpcMediator;
using MediatR;

namespace Example.Server.Handlers;

public class ResultRequestHandler : IRequestHandler<ResultRequest, Result<Dictionary<string, string>>>
{
    public async Task<Result<Dictionary<string, string>>> Handle(ResultRequest request, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);

        if (request.ShouldThrowError)
        {
            return new Result<Dictionary<string, string>>(new Exception("some error"));
        }

        return new Dictionary<string, string>
        {
            { request.Name, request.Value }
        };
    }
}
