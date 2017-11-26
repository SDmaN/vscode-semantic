using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Client
{
    public class RegistrationParams
    {
        [JsonProperty("registrations")]
        public IEnumerable<Registration> Registrations { get; set; }
    }
}