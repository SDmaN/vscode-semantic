
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument.Completion
{
    public class CompletionItem
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("kind")]
        public CompletionItemKind Kind { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("documentation")]
        public MarkupContent Documentation { get; set; }

        [JsonProperty("sortText")]
        public string SortText { get; set; }

        [JsonProperty("filterText")]
        public string FilterText { get; set; }

        [JsonProperty("insertTextFormat")]
        public InsertTextFormat? InsertTextFormat { get; set; }

        [JsonProperty("textEdit")]
        public TextEdit TextEdit { get; set; }

        [JsonProperty("additionalTextEdits")]
        public IEnumerable<TextEdit> AdditionalTextEdits { get; set; }

        [JsonProperty("commitCharacters")]
        public IEnumerable<string> CommitCharacters { get; set; }

        [JsonProperty("command")]
        public Command Command { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}