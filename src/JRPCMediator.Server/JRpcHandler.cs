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

    private readonly HttpContext context;
    private readonly IMediator mediator;
    private readonly IAuthorizationService? authorization;

    public JRpcHandler(HttpContext context, IMediator mediator, IAuthorizationService? authorization)
    {
        this.context = context;
        this.mediator = mediator;
        this.authorization = authorization;
    }
    public JRpcHandler(IHttpContextAccessor accessor, IMediator mediator, IAuthorizationService? authorization)
    {
        this.context = accessor.HttpContext!;
        this.mediator = mediator;
        this.authorization = authorization;
    }
    public static JRpcHandler CreateHandler(HttpContext ctx, IServiceProvider provider)
            => new JRpcHandler(ctx, provider.GetRequiredService<IMediator>(), provider.GetService<IAuthorizationService>());

    private async Task<bool> Authorize(Type requestType)
    {
        var authAttribute = requestType.GetCustomAttribute<JRpcAuthorizeAttribute>();

        if (authAttribute is null || authorization is null) return true;

        if (authAttribute.Roles != null)
            foreach (var role in authAttribute.Roles)
                if (context.User.IsInRole(role))
                    return true;

        if (authAttribute.Policies != null)
            foreach (var policy in authAttribute.Policies)
                if ((await authorization.AuthorizeAsync(context.User, policy)).Succeeded)
                    return true;

        return false;
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

            // authorize
            if (!await Authorize(requestType))
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

    private async Task HandleNotification(JRpcRequest rpcRequest)
    {
        try
        {
            // get request type for method
            if (!Methods.TryGetValue(rpcRequest.Method, out var requestType)) return;

            if (!await Authorize(requestType)) return;

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

    private async Task<JRpcResponse[]> HandleBatchRequest(JRpcRequest[] requests)
    {
        await Task.WhenAll(requests.Where(x => x.IsNotification()).Select(x => HandleNotification(x)));

        return await Task.WhenAll(requests.Where(x => x.IsRequest()).Select(x => HandleRequest(x)));
    }

    public async Task InvokeAsync()
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