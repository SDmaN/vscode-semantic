using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class DocumentFilter
    {
        [JsonProperty("language", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Language { get; set; }

        [JsonProperty("scheme", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Scheme { get; set; }

        [JsonProperty("pattern", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Pattern { get; set; }
    }
}