using JRpcMediator.Models;
using JRpcMediator.Server.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JRpcMediator.Server;

public class JRpcMiddleware
{
    private readonly RequestDelegate next;
    private readonly JRpcServerOptions options;

    public JRpcMiddleware(RequestDelegate next, IOptions<JRpcServerOptions> options)
    {
        this.next = next;
        this.options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, JRpcRequestHandler requestHandler, JRpcNotificationHandler notificationHandler)
    {
        if (context.Request.Method != "POST" || context.Request.Path.Value != options.Route)
        {
            await next(context);
            return;
        }

        try
        {
            var body = await context.Request.ReadFromJsonAsync<JsonElement>();

            object? response = null;
            switch (body.ValueKind)
            {
                case JsonValueKind.Array:
                    var requests = body.Deserialize<JRpcRequest[]>(options.JsonOptions);
                    // when null invalid request
                    if (requests == null)
                    {
                        response = JRpcResponse.InvalidRequest();
                    }
                    // else handle requests
                    else
                    {
                        await Task.WhenAll(requests.Where(x => x.IsNotification()).Select(x => notificationHandler.Handle(context, x)));

                        response = await Task.WhenAll(requests.Where(x => x.IsRequest()).Select(x => requestHandler.Handle(context, x)));

                        context.Response.StatusCode = 200;
                    }
                    break;
                case JsonValueKind.Object:
                    var request = body.Deserialize<JRpcRequest>(options.JsonOptions);
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
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(JRpcResponse.Failure(-1, e));
        }
    }
}