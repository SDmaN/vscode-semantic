using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Window
{
    public class ShowMessageParams
    {
        [JsonProperty("type")]
        public MessageType Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}