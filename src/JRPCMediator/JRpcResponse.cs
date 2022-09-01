﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace JRpcMediator
{
    public class JRpcResponse
    {
        [JsonPropertyName("jsonrpc")]
        public string Version { get; set; } = "2.0";

        [JsonPropertyName("result")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonElement? Result { get; set; }

        [JsonPropertyName("error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JRpcError? Error { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        public static JRpcResponse Success(int id, JsonElement result) => new()
        {
            Id = id,
            Result = result,
        };
        public static JRpcResponse Failure(int id, Exception e) => new()
        {
            Id = id,
            Error = new JRpcError(e),
        };
        public static JRpcResponse InvalidRequest() => Failure(-1, new ArgumentException("invalid request"));
    }
}
