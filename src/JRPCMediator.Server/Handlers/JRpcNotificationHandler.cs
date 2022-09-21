using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace JRpcMediator.Server.Handlers;

public class JRpcNotificationHandler
{
    private readonly IMediator mediator;
    private readonly JRpcAuthorizationHandler authorization;
    public JRpcNotificationHandler(IMediator mediator, JRpcAuthorizationHandler authorization)
    {
        this.mediator = mediator;
        this.authorization = authorization;
    }

    public async Task Handle(HttpContext context, JRpcRequest rpcRequest)
    {
        try
        {
            // get request type for method
            if (!JRpcHandler.Methods.TryGetValue(rpcRequest.Method, out var requestType)) return;

            if (!await authorization.Handle(context, requestType)) return;

            // deserialize params to request
            var notification = rpcRequest.Params.Deserialize(requestType);

            if (notification != null)
            {
                // publish notification
                await mediator.Publish(notification);
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("@{0}: {1}\n{2}", e.GetType().Name, e.Message, e.StackTrace);
        }
    }
}