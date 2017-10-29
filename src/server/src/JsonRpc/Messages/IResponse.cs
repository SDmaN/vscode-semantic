using Newtonsoft.Json;

namespace JsonRpc.Messages
{
    public interface IResponse : IWithId
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        object Result { get; }

        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        Error Error { get; }
    }

    public interface IResponse<out TResult> : IResponse where TResult : class
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        new TResult Result { get; }
    }
}