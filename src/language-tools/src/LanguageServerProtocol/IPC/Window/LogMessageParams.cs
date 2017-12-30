using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Window
{
    public class LogMessageParams
    {
        [JsonProperty("type")]
        public MessageType Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}