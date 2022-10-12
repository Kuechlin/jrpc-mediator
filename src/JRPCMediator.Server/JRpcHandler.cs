using JRpcMediator.Server.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace JRpcMediator.Server;

public class JRpcHandler
{
    public static ConcurrentDictionary<string, Type> Methods = new();
    public static JsonSerializerOptions JsonOptions = new();

    private readonly JRpcRequestHandler requestHandler;
    private readonly JRpcNotificationHandler notificationHandler;
    private readonly JRpcBatchRequestHandler batchRequestHandler;

    public JRpcHandler(JRpcRequestHandler requestHandler, JRpcNotificationHandler notificationHandler, JRpcBatchRequestHandler batchRequestHandler)
    {
        this.requestHandler = requestHandler;
        this.notificationHandler = notificationHandler;
        this.batchRequestHandler = batchRequestHandler;
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
                    var requests = body.Deserialize<JRpcRequest[]>(JsonOptions);
                    // when null invalid request
                    if (requests == null)
                    {
                        response = JRpcResponse.InvalidRequest();
                    }
                    // else handle requests
                    else
                    {
                        response = await batchRequestHandler.Handle(context, requests);
                    }
                    break;
                case JsonValueKind.Object:
                    var request = body.Deserialize<JRpcRequest>(JsonOptions);
                    // when null invalid request
                    if (request == null)
                    {
                        response = JRpcResponse.InvalidRequest();
                    }
                    // when id exists it's a request
                    else if (request.IsRequest())
                    {
                        response = await requestHandler.Handle(context, request);
                    }
                    // else a notification
                    else
                    {
                        await notificationHandler.Handle(context, request);
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
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(JRpcResponse.Failure(-1, e));
        }
    }
}