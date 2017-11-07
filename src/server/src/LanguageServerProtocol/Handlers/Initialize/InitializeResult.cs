using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.Initialize
{
    public class InitializeResult
    {
        [JsonProperty("capabilities")]
        public ServerCapabilities Capabilities { get; set; }
    }

    public class ServerCapabilities
    {
        [JsonProperty("textDocumentSync")]
        public TextDocumentSyncOptions TextDocumentSync { get; set; }

        [JsonProperty("hoverProvider")]
        public bool? HoverProvider { get; set; }

        [JsonProperty("completionProvider")]
        public CompletionOptions CompletionProvider { get; set; }

        [JsonProperty("signatureHelpProvider")]
        public SignatureHelpOptions SignatureHelpProvider { get; set; }

        [JsonProperty("definitionProvider")]
        public bool? DefinitionProvider { get; set; }

        [JsonProperty("referencesProvider")]
        public bool? ReferencesProvider { get; set; }

        [JsonProperty("documentHighlightProvider")]
        public bool? DocumentHighlightProvider { get; set; }

        [JsonProperty("documentSymbolProvider")]
        public bool? DocumentSymbolProvider { get; set; }

        [JsonProperty("workspaceSymbolProvider")]
        public bool? WorkspaceSymbolProvider { get; set; }

        [JsonProperty("codeActionProvider")]
        public bool? CodeActionProvider { get; set; }

        [JsonProperty("codeLensProvider")]
        public bool? CodeLensProvider { get; set; }

        [JsonProperty("documentFormattingProvider")]
        public bool? DocumentFormattingProvider { get; set; }

        [JsonProperty("documentRangeFormattingProvider")]
        public bool? DocumentRangeFormattingProvider { get; set; }

        [JsonProperty("documentOnTypeFormattingProvider")]
        public bool? DocumentOnTypeFormattingProvider { get; set; }

        [JsonProperty("renameProvider")]
        public bool? RenameProvider { get; set; }

        [JsonProperty("documentLinkProvider")]
        public DocumentLinkOptions DocumentLinkProvider { get; set; }

        [JsonProperty("executeCommandProvider")]
        public ExecuteCommandOptions ExecuteCommandProvider { get; set; }

        [JsonProperty("experimental")]
        public object Experimental { get; set; }
    }

    public class TextDocumentSyncOptions
    {
        [JsonProperty("openClose")]
        public bool? OpenClose { get; set; }

        [JsonProperty("change")]
        public TextDocumentSyncKind? Change { get; set; }

        [JsonProperty("willSave")]
        public bool? WillSave { get; set; }

        [JsonProperty("willSaveWaitUntil")]
        public bool? WillSaveWaitUntil { get; set; }

        [JsonProperty("save")]
        public SaveOptions Save { get; set; }
    }

    public enum TextDocumentSyncKind
    {
        None = 0,
        Full = 1,
        Incremental = 2
    }

    public class SaveOptions
    {
        [JsonProperty("includeText")]
        public bool? IncludeText { get; set; }
    }

    public class CompletionOptions
    {
        [JsonProperty("resolveProvider")]
        public bool? ResolveProvider { get; set; }

        [JsonProperty("triggerCharacters")]
        public IList<string> TriggerCharacters { get; set; }
    }

    public class SignatureHelpOptions
    {
        [JsonProperty("triggerCharacters")]
        public IList<string> TriggerCharacters { get; set; }
    }

    public class DocumentOnTypeFormattingOptions
    {
        [JsonProperty("firstTriggerCharacter")]
        public string FirstTriggerCharacter { get; set; }

        [JsonProperty("moreTriggerCharacter")]
        public IList<string> MoreTriggerCharacter { get; set; }
    }

    public class DocumentLinkOptions
    {
        [JsonProperty("resolveProvider")]
        public bool? ResolveProvider { get; set; }
    }

    public class ExecuteCommandOptions
    {
        [JsonProperty("commands")]
        public IList<string> Commands { get; set; }
    }
}