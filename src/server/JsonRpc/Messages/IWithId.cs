using JsonRpc.Converters;
using Newtonsoft.Json;

namespace JsonRpc.Messages
{
    public interface IWithId
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(MessageIdConverter))]
        MessageId Id { get; }
    }
}