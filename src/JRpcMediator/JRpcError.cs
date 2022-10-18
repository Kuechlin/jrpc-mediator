using System.Text.Json.Serialization;

namespace JRpcMediator
{
    public class JRpcError
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("inner")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JRpcError? Inner { get; set; }

        public JRpcError() { }
        public JRpcError(Exception e)
        {
            Type = e.GetType().Name;
            Message = e.Message;

            if (e.InnerException!=null)
            {
                Inner = new JRpcError(e.InnerException);
            }
        }
    }
}
