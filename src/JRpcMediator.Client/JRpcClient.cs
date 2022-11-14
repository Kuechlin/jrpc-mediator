using JRpcMediator.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using static JRpcMediator.Utils.JRpcUtils;

namespace JRpcMediator.Client
{
    public class JRpcClientOptions
    {
        public string Url { get; set; } = string.Empty;
        public JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions();
    }

    public class JRpcClient
    {
        private readonly HttpClient client;
        private readonly JRpcClientOptions options;

        public JRpcClient(string url, HttpClient? client = null)
        {
            options = new JRpcClientOptions { Url = url };
            this.client = client ?? new HttpClient();
        }

        public JRpcClient(JRpcClientOptions options, HttpClient? client = null)
        {
            this.options = options;
            this.client = client ?? new HttpClient();
        }

        public void Configure(Action<HttpClient> setupAction)
        {
            setupAction(client);
        }

        public async Task<TResponse?> Send<TResponse>(IRequest<TResponse> request)
        {
            var rpcReqeust = new JRpcRequest(
                IdUtil.NextId(),
                GetMethod(request.GetType()),
                JsonSerializer.SerializeToElement(request, request.GetType(), options.JsonOptions)
            );

            var response = await client.PostAsJsonAsync(options.Url, rpcReqeust, options.JsonOptions);

            // When Response has no Json Content
            if (response.Content.Headers.ContentType?.MediaType != "application/json")
            {
                var content = response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"request failed: {content}");
            }

            var rpcResponse = await response.Content.ReadFromJsonAsync<JRpcResponse>(options.JsonOptions);

            // handle result types
            if (IsResult(typeof(TResponse)))
            {
                if (rpcResponse is null)
                {
                    return ResultFactory.Create<TResponse>(new InvalidOperationException("Invalid Response"));
                }
                else if (rpcResponse.Error != null)
                {
                    return ResultFactory.Create<TResponse>(rpcResponse.Error.ToException());
                }
                else if (rpcResponse.Result is null)
                {
                    return ResultFactory.Create<TResponse>(new InvalidOperationException("Invalid Response"));
                }
                var result = rpcResponse.Result.Value.Deserialize(GetValueType(typeof(TResponse)), options.JsonOptions);

                return ResultFactory.Create<TResponse>(result);
            }
            // handle normal types
            else
            {
                if (rpcResponse is null)
                {
                    throw new InvalidOperationException("Invalid Response");
                }
                else if (rpcResponse.Error != null)
                {
                    throw rpcResponse.Error.ToException();
                }
                else if (rpcResponse.Result is null)
                {
                    throw new InvalidOperationException("Invalid Response");
                }

                return rpcResponse.Result.Value.Deserialize<TResponse>(options.JsonOptions);
            }
        }

        public async Task Publish(INotification notification)
        {
            var rpcReqeust = new JRpcRequest(
                GetMethod(notification.GetType()),
                JsonSerializer.SerializeToElement(notification, notification.GetType(), options.JsonOptions)
            );

            var response = await client.PostAsJsonAsync(options.Url, rpcReqeust, options.JsonOptions);

            // todo handle result
            if (response.StatusCode == HttpStatusCode.NoContent) return;

            throw new NotImplementedException();
        }

        public async Task<Dictionary<int, IResult>> Batch(Dictionary<int, IBaseRequest> batch)
        {
            var requests = batch
                .Select((x) => new JRpcRequest(
                    x.Key,
                    GetMethod(x.Value.GetType()),
                    JsonSerializer.SerializeToElement(x.Value, x.Value.GetType(), options.JsonOptions)
                ))
                .ToArray();

            var response = await client.PostAsJsonAsync(options.Url, requests, options.JsonOptions);

            // When Response has no Json Content
            if (response.Content.Headers.ContentType?.MediaType != "application/json")
            {
                var content = response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Request failed: {content}");
            }

            var responses = await response.Content.ReadFromJsonAsync<JRpcResponse[]>(options.JsonOptions);

            if (responses is null)
            {
                throw new InvalidOperationException("Invalid Response");
            }

            return responses
                .Select(response =>
                {
                    // check if response id is in batch
                    if (batch.TryGetValue(response.Id, out var request) is false)
                    {
                        return KeyValuePair.Create(response.Id, ResultFactory.Create(typeof(object), new InvalidOperationException($"No Request with Id: {response.Id} found")));
                    }
                    // get return type from request
                    var returnType = GetReturnType(request.GetType());

                    // check if response is error
                    if (response.Error != null)
                    {
                        return KeyValuePair.Create(response.Id, ResultFactory.Create(returnType, response.Error.ToException()));
                    }

                    object? value;
                    // deserialize response
                    if (IsResult(returnType))
                    {
                        value = response.Result?.Deserialize(GetValueType(returnType), options.JsonOptions);
                    }
                    else 
                    {
                        value = response.Result?.Deserialize(returnType, options.JsonOptions);
                    }
                    
                    // check if response value is not null
                    if (value is null)
                    {
                        return KeyValuePair.Create(response.Id, ResultFactory.Create(returnType, new InvalidOperationException("Invalid Response")));
                    }
                    // create success result
                    return KeyValuePair.Create(response.Id, ResultFactory.Create(returnType, value));
                })
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
