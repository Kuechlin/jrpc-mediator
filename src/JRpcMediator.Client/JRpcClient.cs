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

        private HttpClient GetHttpClient()
        {
            if (_client != null) return _client;

            var socketsHandler = new SocketsHttpHandler
            {
            };

            _client = new HttpClient(socketsHandler);

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
    }
}