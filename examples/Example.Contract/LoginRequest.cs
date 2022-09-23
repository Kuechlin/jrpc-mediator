using JRpcMediator;
using MediatR;

namespace Example.Contract;

[JRpcMethod("login")]
public class LoginRequest : IRequest<string>
{
    public LoginRequest(string name, string pass)
    {
        Name = name;
        Pass = pass;
    }

    public string Name { get; set; }
    public string Pass { get; set; }
}