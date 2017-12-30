using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class TextEdit
    {
        [JsonProperty("range")]
        public Range Range { get; set; }

        [JsonProperty("newText")]
        public string NewText { get; set; }
    }
}