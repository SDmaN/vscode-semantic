using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Window
{
    public class ShowMessageRequestParams
    {
        [JsonProperty("type")]
        public MessageType Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("actions")]
        public IList<MessageActionItem> Actions { get; set; }
    }
}