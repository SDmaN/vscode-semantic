using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class WorkspaceEdit
    {
        [JsonProperty("changes")]
        public IDictionary<string, IList<TextEdit>> Changes { get; set; }

        [JsonProperty("documentChanges")]
        public IList<TextDocumentEdit> DocumentChanges { get; set; }
    }
}