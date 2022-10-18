using JRpcMediator.Server.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;
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
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new JRpcNotFoundException("Method not found"));
            }

            // authenticate
            if (!await authentication.Handle(context, requestType))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new JRpcUnauthorizedException());
            }

            // authorize
            if (!await authorization.Handle(context, requestType))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new JRpcUnauthorizedException());
            }

            // deserialize params to request
            var request = rpcRequest.Params.Deserialize(requestType, JRpcHandler.JsonOptions);

            if (request is null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new JRpcBadRequestException("Invalid Request"));
            }

            // send request
            var response = await mediator.Send(request);

            // serialize result
            var responseBody = JsonSerializer.SerializeToElement(response, JRpcHandler.JsonOptions);

            // write response
            return JRpcResponse.Success(rpcRequest.Id!.Value, responseBody);
        }
        // handle exceptions
        catch (JRpcBadRequestException e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return JRpcResponse.Failure(rpcRequest.Id!.Value, e);
        }
        catch (JRpcNotFoundException e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return JRpcResponse.Failure(rpcRequest.Id!.Value, e);
        }
        catch (JRpcUnauthorizedException e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return JRpcResponse.Failure(rpcRequest.Id!.Value, e);
        }
        catch (Exception e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return JRpcResponse.Failure(rpcRequest.Id!.Value, e);
        }
    }
}
