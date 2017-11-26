using System.Collections.Generic;
using LanguageServerProtocol.Handlers.TextDocument;
using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Client.RegistrationOptions
{
    public class TextDocumentRegistrationOptions
    {
        [JsonProperty("documentSelector")]
        public IEnumerable<DocumentFilter> DocumentSelector { get; set; }
    }
}