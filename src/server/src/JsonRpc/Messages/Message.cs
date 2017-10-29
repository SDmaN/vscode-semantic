using Newtonsoft.Json;

namespace JsonRpc.Messages
{
    public abstract class Message
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; } = "2.0";
    }
}