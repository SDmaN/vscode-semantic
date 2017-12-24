using JsonRpc.Converters;
using Newtonsoft.Json;

namespace JsonRpc.Messages
{
    public interface IWithId
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(MessageIdConverter))]
        MessageId Id { get; }
    }
}