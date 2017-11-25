using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class Position
    {
        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("character")]
        public int Character { get; set; }
    }
}