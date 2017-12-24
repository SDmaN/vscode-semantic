using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class Range
    {
        [JsonProperty("start")]
        public Position Start { get; set; }

        [JsonProperty("end")]
        public Position End { get; set; }
    }
}