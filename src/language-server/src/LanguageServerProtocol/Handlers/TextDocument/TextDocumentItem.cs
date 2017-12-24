using System;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class TextDocumentItem
    {
        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("languageId")]
        public string LanguageId { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}