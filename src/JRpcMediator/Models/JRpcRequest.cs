using System.Text.Json;
using System.Text.Json.Serialization;

namespace JRpcMediator.Models;

public class JRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string Version { get; set; } = "2.0";

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("params")]
    public JsonElement Params { get; set; }

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; set; }

    public bool IsRequest() => Id.HasValue;
    public bool IsNotification() => Id is null;

    public JRpcRequest() { }
    public JRpcRequest(int id, string method, JsonElement @params)
    {
        Id = id;
        Method = method;
        Params = @params;
    }
    public JRpcRequest(string method, JsonElement @params)
    {
        Method = method;
        Params = @params;
    }
}