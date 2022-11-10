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

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
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

            if (rpcResponse is null)
            {
                throw new InvalidOperationException($"response is null");
            }

            if (rpcResponse.Error != null)
            {
                throw rpcResponse.Error.ToException();
            }

            return rpcResponse.Result is null
                ? default
                : rpcResponse.Result.Value.Deserialize<TResponse>(options.JsonOptions);
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

        public async Task<Dictionary<int, Result>> Batch(Dictionary<int, IBaseRequest> batch)
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
                throw new InvalidOperationException($"request failed: {content}");
            }

            var responses = await response.Content.ReadFromJsonAsync<JRpcResponse[]>(options.JsonOptions);

            if (responses is null)
            {
                throw new InvalidOperationException($"response is null");
            }

            return responses
                .Select(response =>
                {
                    // check if response is error
                    if (response.Error != null)
                    {
                        return KeyValuePair.Create(response.Id, new Result(response.Error.ToException()));
                    }
                    // check if response id is in batch
                    if (batch.TryGetValue(response.Id, out var request) is false)
                    {
                        return KeyValuePair.Create(response.Id, new Result(new InvalidOperationException($"No Request with Id: {response.Id} found")));
                    }
                    // deserialize response
                    var returnType = GetReturnType(request.GetType());

                    var value = response.Result?.Deserialize(returnType, options.JsonOptions);
                    // check if response value is not null
                    if (value is null)
                    {
                        return KeyValuePair.Create(response.Id, new Result(new InvalidOperationException("Invalid Response")));
                    }
                    // create success result
                    return KeyValuePair.Create(response.Id, new Result(value));
                })
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
