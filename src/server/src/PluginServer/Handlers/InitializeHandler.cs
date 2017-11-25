using System;
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
            await _windowMessageSender.LogMessage(new LogMessageParams
            {
                Message = "Plugin server initialized.",
                Type = MessageType.Info
            });

            InitializeResult initResult = new InitializeResult
            {
                Capabilities = new ServerCapabilities
                {
                    HoverProvider = true,
                    RenameProvider = true,
                    TextDocumentSync = new TextDocumentSyncOptions
                    {
                        OpenClose = true,
                        Change = TextDocumentSyncKind.Incremental,
                        WillSave = false,
                        WillSaveWaitUntil = true,
                        Save = new SaveOptions { IncludeText = true }
                    }
                }
            };

            return Ok(initResult);
        }
    }
}