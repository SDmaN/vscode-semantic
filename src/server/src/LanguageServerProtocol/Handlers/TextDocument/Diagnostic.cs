using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class Diagnostic
    {
        [JsonProperty("range")]
        public Range Range { get; set; }

        [JsonProperty("severity")]
        public DiagnosticSeverity Severity { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}