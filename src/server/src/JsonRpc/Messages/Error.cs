using Newtonsoft.Json;

namespace JsonRpc.Messages
{
    public class Error
    {
        public Error(ErrorCode code, string message)
            : this(code, message, null)
        {
            Code = code;
            Message = message;
        }

        [JsonConstructor]
        public Error([JsonProperty("code")] ErrorCode code, [JsonProperty("message")] string message,
            [JsonProperty("data")] object data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        [JsonProperty("code")]
        public ErrorCode Code { get; }

        [JsonProperty("message")]
        public string Message { get; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; }
    }
}