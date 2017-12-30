using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        [JsonProperty("version")]
        public long Version { get; set; }
    }
}