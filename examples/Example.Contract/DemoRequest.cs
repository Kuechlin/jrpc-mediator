using JRpcMediator;
using MediatR;

namespace Example.Contract;

[JRpcMethod("demo")]
public class DemoRequest : IRequest<string>
{
    public DemoRequest(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}

