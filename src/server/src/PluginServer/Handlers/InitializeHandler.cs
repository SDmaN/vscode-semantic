using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Handlers.Initialize;
using LanguageServerProtocol.IPC.Window;

namespace PluginServer.Handlers
{
    public class InitializeHandler : DefaultInitializeHandler
    {
        private readonly IWindowMessageSender _windowMessageSender;

        public InitializeHandler(IWindowMessageSender windowMessageSender)
        {
            _windowMessageSender = windowMessageSender;
        }

        public override async Task<IRpcHandleResult<InitializeResult>> Handle(long processId, string rootPath,
            Uri rootUri, ClientCapabilities capabilities, string trace)
        {
            await _windowMessageSender.ShowMessage(MessageType.Warning, "Init");

            InitializeResult initResult = new InitializeResult
            {
                Capabilities = new ServerCapabilities
                {
                    HoverProvider = false,
                    RenameProvider = false,
                    TextDocumentSync = new TextDocumentSyncOptions
                    {
                        OpenClose = true,
                        Change = TextDocumentSyncKind.Incremental,
                        WillSave = false,
                        WillSaveWaitUntil = true,
                        Save = new SaveOptions { IncludeText = true }
                    },
                    CompletionProvider = new CompletionOptions
                    {
                        TriggerCharacters = new List<string>
                        {
                            "f",
                            "g"
                        },
                        ResolveProvider = true
                    }
                }
            };

            await _windowMessageSender.LogMessage(MessageType.Info, "Plugin server initialized");

            return Ok(initResult);
        }
    }
}