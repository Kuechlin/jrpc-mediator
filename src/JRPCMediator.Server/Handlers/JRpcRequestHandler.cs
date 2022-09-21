using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace JRpcMediator.Server.Handlers;

public class JRpcRequestHandler
{
    private readonly IMediator mediator;
    private readonly JRpcAuthorizationHandler authorization;
    public JRpcRequestHandler(IMediator mediator, JRpcAuthorizationHandler authorization)
    {
        this.mediator = mediator;
        this.authorization = authorization;
    }

    public async Task<JRpcResponse> Handle(HttpContext context, JRpcRequest rpcRequest)
    {
        try
        {
            // get request type for method
            if (!JRpcHandler.Methods.TryGetValue(rpcRequest.Method, out var requestType))
            {
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new InvalidOperationException("method not found"));
            }

            // authorize
            if (!await authorization.Handle(context, requestType))
            {
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new JRpcUnauthorizedAccessException());
            }

            // deserialize params to request
            var request = rpcRequest.Params.Deserialize(requestType);

            if (request is null)
            {
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
            // return error
            return JRpcResponse.Failure(rpcRequest.Id!.Value, e);
        }
    }
}
