using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Text.Json;

namespace JRpcMediator.Server;

public class JRpcHandler
{
    public static ConcurrentDictionary<string, Type> Methods = new();

    private readonly IMediator mediator;

    public JRpcHandler(IMediator mediator)
    {
        this.mediator = mediator;
    }

    private async Task<JRpcResponse> HandleRequest(JRpcRequest rpcRequest)
    {
        try
        {
            // get request type for method
            if (!Methods.TryGetValue(rpcRequest.Method, out var requestType))
            {
                return JRpcResponse.Failure(rpcRequest.Id!.Value, new InvalidOperationException("method not found"));
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

    private async Task HandleNotification(JRpcRequest rpcRequest)
    {
        try
        {
            // get request type for method
            if (Methods.TryGetValue(rpcRequest.Method, out var requestType))
            {
                // deserialize params to request
                var notification = rpcRequest.Params.Deserialize(requestType);

                if (notification != null)
                {
                    // publish notification
                    await mediator.Publish(notification);
                }
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("@{0}: {1}\n{2}", e.GetType().Name, e.Message, e.StackTrace);
        }
    }

    private async Task<JRpcResponse[]> HandleBatchRequest(JRpcRequest[] requests)
    {
        await Task.WhenAll(requests.Where(x => x.IsNotification()).Select(x => HandleNotification(x)));

        return await Task.WhenAll(requests.Where(x => x.IsRequest()).Select(x => HandleRequest(x)));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var body = await context.Request.ReadFromJsonAsync<JsonElement>();

            object? response = null;
            switch (body.ValueKind)
            {
                case JsonValueKind.Array:
                    var requests = body.Deserialize<JRpcRequest[]>();
                    // when null invalid request
                    if (requests == null)
                    {
                        response = JRpcResponse.InvalidRequest();
                    }
                    // else handle requests
                    else
                    {
                        response = await HandleBatchRequest(requests);
                    }
                    break;
                case JsonValueKind.Object:
                    var request = body.Deserialize<JRpcRequest>();
                    // when null invalid request
                    if (request == null)
                    {
                        response = JRpcResponse.InvalidRequest();
                    }
                    // when id exists it's a request
                    else if (request.IsRequest())
                    {
                        response = await HandleRequest(request);
                    }
                    // else a notification
                    else
                    {
                        await HandleNotification(request);
                    }
                    break;
                default:
                    // invalid request
                    response = JRpcResponse.InvalidRequest();
                    break;
            }

            // write no content response
            if (response is null)
            {
                context.Response.StatusCode = 201;
                await context.Response.CompleteAsync();
            }
            // write response
            else
            {
                await context.Response.WriteAsJsonAsync(response, response.GetType());
            }
        }
        catch (Exception e)
        {
            await context.Response.WriteAsJsonAsync(JRpcResponse.Failure(-1, e));
        }
    }
}