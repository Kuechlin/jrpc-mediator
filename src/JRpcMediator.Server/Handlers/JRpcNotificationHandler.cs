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
            if (!JRpcHandler.Methods.TryGetValue(rpcRequest.Method, out var requestType))
            {
                context.Response.StatusCode = 400;
                return;
            }

            if (!await authorization.Handle(context, requestType))
            {
                context.Response.StatusCode = 401;
                return;
            }

            // deserialize params to request
            var notification = rpcRequest.Params.Deserialize(requestType, JRpcHandler.JsonOptions);

            if (notification == null)
            {
                context.Response.StatusCode = 400;
                return;
            }

            // publish notification
            await mediator.Publish(notification);
            context.Response.StatusCode = 201;
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            Console.Error.WriteLine("@{0}: {1}\n{2}", e.GetType().Name, e.Message, e.StackTrace);
        }
    }
}