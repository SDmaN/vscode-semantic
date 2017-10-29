using Newtonsoft.Json;

namespace JsonRpc.Messages
{
    public class Error
    {
        public Error(ErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public Error(ErrorCode code, string message, object data)
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