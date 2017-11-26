using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument.Completion
{
    public class CompletionContext
    {
        [JsonProperty("triggerKind")]
        public CompletionTriggerKind TriggerKind { get; set; }

        [JsonProperty("triggerCharacter")]
        public string TriggerCharacter { get; set; }
    }
}