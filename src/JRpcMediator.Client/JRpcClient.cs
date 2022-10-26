using JRpcMediator.Client.Models;
using JRpcMediator.Models;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using static JRpcMediator.Utils.JRpcUtils;

namespace JRpcMediator.Client
{
    public class JRpcClientOptions
    {
        public string Url { get; set; } = string.Empty;
        public JsonSerializerOptions JsonOptions { get; set; } = new();
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

        public async Task<BatchResult[]> Batch(IEnumerable<BatchRequest> batch)
        {
            var requests = batch
                .Select(x => new JRpcRequest(
                    x.Id,
                    GetMethod(x.Request.GetType()),
                    JsonSerializer.SerializeToElement(x.Request, x.Request.GetType(), options.JsonOptions)
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

            return responses.Select(response =>
            {
                if (response.Error != null)
                {
                    return new BatchResult(response.Id, response.Error.ToException());
                }
                var batchRequest = batch.FirstOrDefault(x => x.Id == response.Id);
                if (batchRequest is null)
                {
                    return new BatchResult(response.Id, new InvalidOperationException("no request with id found"));
                }
                else
                {
                    var returnType = GetReturnType(batchRequest.Request.GetType());

                    return new BatchResult(response.Id, response.Result?.Deserialize(returnType, options.JsonOptions));
                }
            }).ToArray();
        }
    }
}
