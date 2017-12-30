using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument.DidChange
{
    public class TextDocumentContentChangeEvent
    {
        [JsonProperty("range", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Range Range { get; set; }

        [JsonProperty("rangeLength", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? RangeLength { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}