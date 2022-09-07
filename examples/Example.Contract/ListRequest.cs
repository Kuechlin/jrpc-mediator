using JRpcMediator;
using MediatR;

namespace Example.Contract;

[JRpcMethod("list")]
public class ListRequest : IRequest<int[]>
{
    public ListRequest(int length)
    {
        Length = length;
    }

    public int Length { get; }
}