using System;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class Location
    {
        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("range")]
        public Range Range { get; set; }
    }
}