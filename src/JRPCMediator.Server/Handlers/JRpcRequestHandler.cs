using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace JRpcMediator.Server.Handlers;

public class JRpcRequestHandler
{
    private readonly IMediator mediator;
    private readonly JRpcAuthorizationHandler authorization;
    private readonly JRpcAuthenticationHandler authentication;

    public JRpcRequestHandler(IMediator mediator, JRpcAuthorizationHandler authorization, JRpcAuthenticationHandler authentication)
    {
        this.mediator = mediator;
        this.authorization = authorization;
        this.authentication = authentication;
    }

    public async Task<JRpcResponse> Handle(HttpContext context, JRpcRequest rpcRequest)
    {
        try
        {
            // get request type for method
            if (!JRpcHandler.Methods.TryGetValue(rpcRequest.Method, out var requestType))
            {
                context.Response.StatusCode = 400;
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new InvalidOperationException("method not found"));
            }

            // authenticate
            if (!await authentication.Handle(context, requestType))
            {
                context.Response.StatusCode = 401;
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new JRpcUnauthorizedAccessException());
            }

            // authorize
            if (!await authorization.Handle(context, requestType))
            {
                context.Response.StatusCode = 403;
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new JRpcUnauthorizedAccessException());
            }

            // deserialize params to request
            var request = rpcRequest.Params.Deserialize(requestType, JRpcHandler.JsonOptions);

            if (request is null)
            {
                context.Response.StatusCode = 400;
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new ArgumentNullException("invalid request"));
            }

            // send request
            var response = await mediator.Send(request);

            // serialize result
            var responseBody = JsonSerializer.SerializeToElement(response);

            // write response
            return JRpcResponse.Success(rpcRequest.Id!.Value, responseBody);
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            // return error
            return JRpcResponse.Failure(rpcRequest.Id!.Value, e);
        }
    }
}
