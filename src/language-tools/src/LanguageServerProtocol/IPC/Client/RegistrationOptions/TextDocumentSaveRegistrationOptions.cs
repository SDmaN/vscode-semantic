using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Client.RegistrationOptions
{
    public class TextDocumentSaveRegistrationOptions : TextDocumentRegistrationOptions
    {
        [JsonProperty("includeText", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? IncludeText { get; set; }
    }
}