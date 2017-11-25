using System;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class TextDocumentIdentifier
    {
        [JsonProperty("uri")]
        public Uri Uri { get; set; }
    }
}