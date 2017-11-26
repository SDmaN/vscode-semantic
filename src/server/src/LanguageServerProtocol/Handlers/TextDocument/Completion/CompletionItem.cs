using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument.Completion
{
    public class CompletionItem
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("kind", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CompletionItemKind? Kind { get; set; }

        [JsonProperty("detail", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Detail { get; set; }

        [JsonProperty("documentation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public MarkupContent Documentation { get; set; }

        [JsonProperty("sortText", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SortText { get; set; }

        [JsonProperty("filterText", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FilterText { get; set; }

        [JsonProperty("insertTextFormat", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public InsertTextFormat? InsertTextFormat { get; set; }

        [JsonProperty("textEdit", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TextEdit TextEdit { get; set; }

        [JsonProperty("additionalTextEdits", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IEnumerable<TextEdit> AdditionalTextEdits { get; set; }

        [JsonProperty("commitCharacters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IEnumerable<string> CommitCharacters { get; set; }

        [JsonProperty("command", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Command Command { get; set; }

        [JsonProperty("data", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Data { get; set; }
    }
}