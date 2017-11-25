using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class DocumentFilter
    {
        [JsonProperty("language")]
        public string Language { get; set; }
        
        [JsonProperty("scheme")]
        public string Scheme { get; set; }
        
        [JsonProperty("pattern")]
        public string Pattern { get; set; }
    }
}