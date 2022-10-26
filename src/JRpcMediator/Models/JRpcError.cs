using JRpcMediator.Exceptions;
using System.Text.Json.Serialization;

namespace JRpcMediator.Models
{
    public class JRpcError
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = nameof(Exception);
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("inner")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JRpcError? Inner { get; set; }

        public static JRpcError Create(Exception e) => new()
        {
            Type = e.GetType().Name,
            Message = e.Message,
            Inner = e.InnerException != null ? Create(e.InnerException) : null
        };

        public Exception ToException() => Type switch
        {
            nameof(JRpcBadRequestException) => new JRpcBadRequestException(Message, Inner?.ToException()),
            nameof(JRpcNotFoundException) => new JRpcNotFoundException(Message, Inner?.ToException()),
            nameof(JRpcUnauthorizedException) => new JRpcUnauthorizedException(Message, Inner?.ToException()),
            _ => new Exception($"{Type}: {Message}", Inner?.ToException()),
        };
    }
}
