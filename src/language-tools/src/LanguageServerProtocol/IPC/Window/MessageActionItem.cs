using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Window
{
    public class MessageActionItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}