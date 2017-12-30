using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class MarkupContent
    {
        [JsonProperty("kind")]
        public MarkupKind Kind { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}