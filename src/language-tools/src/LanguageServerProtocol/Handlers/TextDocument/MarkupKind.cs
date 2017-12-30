using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarkupKind
    {
        [EnumMember(Value = "plaintext")] PlainText,

        [EnumMember(Value = "markdown")] Markdown
    }
}