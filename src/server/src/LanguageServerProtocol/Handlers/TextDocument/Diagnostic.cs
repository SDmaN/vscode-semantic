using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class Diagnostic
    {
        [JsonProperty("range")]
        public Range Range { get; set; }

        [JsonProperty("severity", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DiagnosticSeverity? Severity { get; set; }

        [JsonProperty("code", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty("source", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}