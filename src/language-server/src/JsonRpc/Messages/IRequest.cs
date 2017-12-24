using Newtonsoft.Json;

namespace JsonRpc.Messages
{
    public interface IRequest : IWithId
    {
        [JsonProperty("method")]
        string Method { get; }

        [JsonProperty("params")]
        object Parameters { get; }
    }

    public interface IRequest<out TParameters> : IRequest
    {
        [JsonProperty("params")]
        new TParameters Parameters { get; }
    }
}