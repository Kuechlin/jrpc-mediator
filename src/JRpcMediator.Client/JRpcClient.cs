using JRpcMediator.Client.Models;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using static JRpcMediator.JRpcUtils;

namespace JRpcMediator.Client
{
    public class JRpcClientOptions
    {
        public string Url { get; init; }
    }

    public class JRpcClient
    {
        private static HttpClient? _client;
        private readonly JRpcClientOptions _options;
        public JRpcClient(string url)
        {
            _options = new JRpcClientOptions { Url = url };
        }
        public JRpcClient(IOptions<JRpcClientOptions> options)
        {
            _options = options.Value;
        }
        public JRpcClient(HttpClient client, string url) : this(url)
        {
            _client = client;
        }

        private HttpClient GetHttpClient()
        {
            if (_client != null) return _client;

            _client = new HttpClient();

            return _client;
        }

        public async Task<TResponse?> Send<TResponse>(IRequest<TResponse> request)
        {
            var rpcReqeust = new JRpcRequest(
                IdUtil.NextId(),
                GetMethod(request.GetType()),
                JsonSerializer.SerializeToElement(request, request.GetType())
            );

            var response = await GetHttpClient().PostAsJsonAsync(_options.Url, rpcReqeust);

            if (response.IsSuccessStatusCode is false)
            {
                var content = response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"request failed: {content}");
            }

            var rpcResponse = await response.Content.ReadFromJsonAsync<JRpcResponse>();

            if (rpcResponse is null)
            {
                throw new InvalidOperationException($"response is null");
            }

            if (rpcResponse.Error != null)
            {
                throw new JRpcException(rpcResponse.Error);
            }

            return rpcResponse.Result is null
                ? default
                : rpcResponse.Result.Value.Deserialize<TResponse>();
        }

        public async Task Publish(INotification notification)
        {
            var rpcReqeust = new JRpcRequest(
                GetMethod(notification.GetType()),
                JsonSerializer.SerializeToElement(notification, notification.GetType())
            );

            var response = await GetHttpClient().PostAsJsonAsync(_options.Url, rpcReqeust);

            if (response.IsSuccessStatusCode is false)
            {
                var content = response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"request failed: {content}");
            }
        }

        public async Task<BatchResult[]> Batch(IEnumerable<BatchRequest> batch)
        {
            var requests = batch
                .Select(x => new JRpcRequest(
                    x.Id,
                    GetMethod(x.Request.GetType()),
                    JsonSerializer.SerializeToElement(x.Request, x.Request.GetType())
                ))
                .ToArray();

            var response = await GetHttpClient().PostAsJsonAsync(_options.Url, requests);

            if (response.IsSuccessStatusCode is false)
            {
                var content = response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"request failed: {content}");
            }

            var responses = await response.Content.ReadFromJsonAsync<JRpcResponse[]>();

            if (responses is null)
            {
                throw new InvalidOperationException($"response is null");
            }

            return responses.Select(response =>
            {
                if (response.Error != null)
                {
                    return new BatchResult(response.Id, new JRpcException(response.Error));
                }
                var batchRequest = batch.FirstOrDefault(x => x.Id == response.Id);
                if (batchRequest is null)
                {
                    return new BatchResult(response.Id, new InvalidOperationException("no request with id found"));
                }
                else
                {
                    var returnType = GetReturnType(batchRequest.Request.GetType());

                    return new BatchResult(response.Id, response.Result?.Deserialize(returnType));
                }

            }).ToArray();
        }
    }
}
