using Newtonsoft.Json;

namespace LanguageServerProtocol.Initialize
{
    public class WorkspaceClientCapabilities
    {
        [JsonProperty("applyEdit")]
        public bool? ApplyEdit { get; set; }

        [JsonProperty("workspaceEdit")]
        public WorkspaceEdit WorkspaceEdit { get; set; }

        [JsonProperty("didChangeConfiguration")]
        public DynamicRegistrationProperty DidChangeConfiguration { get; set; }

        [JsonProperty("didChangeWatchedFiles")]
        public DynamicRegistrationProperty DidChangeWatchedFiles { get; set; }

        [JsonProperty("symbol")]
        public DynamicRegistrationProperty Symbol { get; set; }

        [JsonProperty("executeCommand")]
        public DynamicRegistrationProperty ExecuteCommand { get; set; }
    }

    public class WorkspaceEdit
    {
        [JsonProperty("documentChanges")]
        public bool? DocumentChanges { get; set; }
    }
}