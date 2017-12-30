using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class Command
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("command")]
        public string CommandId { get; set; }

        [JsonProperty("arguments", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<object> Arguments { get; set; }
    }
}