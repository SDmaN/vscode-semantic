using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Client
{
    public class Registration
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("registerOptions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object RegisterOptions { get; set; }
    }
}