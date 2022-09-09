using Example.Contract;
using MediatR;
using System.Text;

namespace Example.Server.Handlers;

public class SecretRequestHandler : IRequestHandler<SecretRequest, string>
{
    public SecretRequestHandler(IHttpContextAccessor accessor)
    {
        Accessor = accessor;
    }

    public IHttpContextAccessor Accessor { get; }

    public Task<string> Handle(SecretRequest request, CancellationToken cancellationToken)
    {
        var name = Accessor.HttpContext?.User?.Identity?.Name;
        return Task.FromResult(name + ": " + Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Text)));
    }
}