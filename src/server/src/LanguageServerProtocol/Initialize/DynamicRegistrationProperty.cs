using Newtonsoft.Json;

namespace LanguageServerProtocol.Initialize
{
    public class DynamicRegistrationProperty
    {
        [JsonProperty("dynamicRegistration")]
        public bool? DynamicRegistration { get; set; }
    }
}