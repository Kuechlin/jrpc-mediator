using Example.Contract;
using JRpcMediator;
using MediatR;

namespace Example.Server.Handlers;

public class DemoRequestHandler : IRequestHandler<DemoRequest, string>
{
    public Task<string> Handle(DemoRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Hallo {request.Name}");
    }
}


