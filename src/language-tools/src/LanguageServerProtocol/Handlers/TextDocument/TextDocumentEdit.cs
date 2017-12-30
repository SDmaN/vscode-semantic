using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class TextDocumentEdit
    {
        [JsonProperty("textDocument")]
        public VersionedTextDocumentIdentifier TextDocument { get; set; }

        [JsonProperty("edits")]
        public IList<TextEdit> Edits { get; set; }
    }
}