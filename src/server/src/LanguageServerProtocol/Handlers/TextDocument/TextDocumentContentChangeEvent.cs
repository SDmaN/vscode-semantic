using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class TextDocumentContentChangeEvent
    {
        [JsonProperty("range")]
        public Range Range { get; set; }

        [JsonProperty("rangeLength")]
        public int RangeLength { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}