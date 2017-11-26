using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Client.RegistrationOptions
{
    public class CompletionRegistrationOptions : TextDocumentRegistrationOptions
    {
        [JsonProperty("triggerCharacters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IEnumerable<string> TriggerCharacters { get; set; }

        [JsonProperty("resolveProvider", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? ResolveProvider { get; set; }
    }
}