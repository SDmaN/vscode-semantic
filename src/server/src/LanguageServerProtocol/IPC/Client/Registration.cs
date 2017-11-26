using System;
using System.Collections.Generic;
using System.Linq;
using LanguageServerProtocol.Handlers.Initialize;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.IPC.Client.RegistrationOptions;
using Newtonsoft.Json;

namespace LanguageServerProtocol.IPC.Client
{
    public class Registration
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("registerOptions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object RegisterOptions { get; set; }

        public static Registration GetDidOpenRegistration(params string[] languages)
        {
            return GetTextDocumentRegistration("textDocument/didOpen", languages);
        }

        public static Registration GetDidChangeRegistration(TextDocumentSyncKind syncKind, params string[] languages)
        {
            return GetRegistration("textDocument/didChange", new TextDocumentChangeRegistrationOptions
            {
                DocumentSelector = GetDocumentSelector(languages),
                SyncKind = syncKind
            });
        }

        public static Registration GetWillSaveRegistration(params string[] languages)
        {
            return GetTextDocumentRegistration("textDocument/willSave", languages);
        }

        public static Registration GetWillSaveWaitUntilRegistration(params string[] languages)
        {
            return GetTextDocumentRegistration("textDocument/willSaveWaitUntil", languages);
        }

        public static Registration GetDidSaveRegistration(bool? includeText, params string[] languages)
        {
            return GetRegistration("textDocument/didSave", new TextDocumentSaveRegistrationOptions
            {
                DocumentSelector = GetDocumentSelector(languages),
                IncludeText = includeText
            });
        }

        public static Registration GetDidCloseRegistration(params string[] languages)
        {
            return GetTextDocumentRegistration("textDocument/didClose", languages);
        }

        public static Registration GetCompletionRegistration(IEnumerable<string> triggerCharacters,
            bool? resolveProvider, params string[] languages)
        {
            return GetRegistration("textDocument/completion", new CompletionRegistrationOptions
            {
                DocumentSelector = GetDocumentSelector(languages),
                TriggerCharacters = triggerCharacters,
                ResolveProvider = resolveProvider
            });
        }

        private static Registration GetTextDocumentRegistration(string method, params string[] languages)
        {
            return GetRegistration(method, new TextDocumentRegistrationOptions
            {
                DocumentSelector = GetDocumentSelector(languages)
            });
        }

        private static Registration GetRegistration(string method, object options)
        {
            return new Registration
            {
                Id = Guid.NewGuid().ToString(),
                Method = method,
                RegisterOptions = options
            };
        }

        private static IEnumerable<DocumentFilter> GetDocumentSelector(params string[] languages)
        {
            return languages.Select(x => new DocumentFilter { Language = x });
        }
    }
}