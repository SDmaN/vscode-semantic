using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class WorkspaceEdit
    {
        [JsonProperty("changes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, IList<TextEdit>> Changes { get; set; }

        [JsonProperty("documentChanges", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<TextDocumentEdit> DocumentChanges { get; set; }
    }
}