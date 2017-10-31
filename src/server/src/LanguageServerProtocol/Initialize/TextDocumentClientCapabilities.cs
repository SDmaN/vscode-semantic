using Newtonsoft.Json;

namespace LanguageServerProtocol.Initialize
{
    public class TextDocumentClientCapabilities
    {
        [JsonProperty("synchronization")]
        public SynchronizationPropery Synchronization { get; set; }

        [JsonProperty("completion")]
        public CompletionProperty Completion { get; set; }

        [JsonProperty("hover")]
        public DynamicRegistrationProperty Hover { get; set; }

        [JsonProperty("signatureHelp")]
        public DynamicRegistrationProperty SignatureHelp { get; set; }

        [JsonProperty("references")]
        public DynamicRegistrationProperty References { get; set; }

        [JsonProperty("documentHighlight")]
        public DynamicRegistrationProperty DocumentHighlight { get; set; }

        [JsonProperty("formatting")]
        public DynamicRegistrationProperty Formatting { get; set; }

        [JsonProperty("rangeFormatting")]
        public DynamicRegistrationProperty RangeFormatting { get; set; }

        [JsonProperty("onTypeFormatting")]
        public DynamicRegistrationProperty OnTypeFormatting { get; set; }

        [JsonProperty("definition")]
        public DynamicRegistrationProperty Definition { get; set; }

        [JsonProperty("codeLens")]
        public DynamicRegistrationProperty CodeLens { get; set; }

        [JsonProperty("documentLink")]
        public DynamicRegistrationProperty DocumentLink { get; set; }
    }

    public class SynchronizationPropery : DynamicRegistrationProperty
    {
        [JsonProperty("willSave")]
        public bool? WillSave { get; set; }

        [JsonProperty("willSaveWaitUntil")]
        public bool? WillSaveWaitUntil { get; set; }

        [JsonProperty("didSave")]
        public bool? DidSave { get; set; }
    }

    public class CompletionProperty : DynamicRegistrationProperty
    {
        [JsonProperty("completionItem")]
        public CompletionItemProperty CompletionItem { get; set; }
    }

    public class CompletionItemProperty
    {
        [JsonProperty("snippetSupport")]
        public bool? SnippetSupport { get; set; }
    }
}