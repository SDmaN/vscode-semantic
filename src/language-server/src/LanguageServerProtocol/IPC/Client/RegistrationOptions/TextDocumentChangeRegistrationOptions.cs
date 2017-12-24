using LanguageServerProtocol.Handlers.Initialize;
using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Client.RegistrationOptions
{
    public class TextDocumentChangeRegistrationOptions : TextDocumentRegistrationOptions
    {
        [JsonProperty("syncKind")]
        public TextDocumentSyncKind SyncKind { get; set; }
    }
}