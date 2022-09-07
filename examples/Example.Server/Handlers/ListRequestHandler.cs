using Example.Contract;
using JRpcMediator;
using MediatR;

namespace Example.Server.Handlers;

public class ListRequestHandler : IRequestHandler<ListRequest, int[]>
{
    public Task<int[]> Handle(ListRequest request, CancellationToken cancellationToken)
    {
        var result = new List<int>();
        for (int i = 0; i < request.Length; i++)
        {
            result.Add(Random.Shared.Next());
        }
        return Task.FromResult(result.ToArray());
    }
}


