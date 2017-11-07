using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.Initialize
{
    public class DynamicRegistrationProperty
    {
        [JsonProperty("dynamicRegistration")]
        public bool? DynamicRegistration { get; set; }
    }
}