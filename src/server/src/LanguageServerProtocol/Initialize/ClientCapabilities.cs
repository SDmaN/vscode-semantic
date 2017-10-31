using Newtonsoft.Json;

namespace LanguageServerProtocol.Initialize
{
    public class ClientCapabilities
    {
        [JsonProperty("workspace")]
        public WorkspaceClientCapabilities Workspace { get; set; }

        [JsonProperty("textDocument")]
        public TextDocumentClientCapabilities TextDocument { get; set; }

        [JsonProperty("experimental")]
        public object Experimental { get; set; }
    }
}