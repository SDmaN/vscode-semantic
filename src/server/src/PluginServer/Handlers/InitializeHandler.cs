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
        private readonly IMessageSender _messageSender;

        public InitializeHandler(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public override async Task<IRpcHandleResult<InitializeResult>> Handle(long processId, string rootPath,
            Uri rootUri, ClientCapabilities capabilities, string trace)
        {
            MessageActionItem a = await _messageSender.ShowMessageRequest(new ShowMessageRequestParams
            {
                Message = "Hi",
                Type = MessageType.Info,
                Actions = new List<MessageActionItem>
                {
                    new MessageActionItem { Title = "A" },
                    new MessageActionItem { Title = "B" }
                }
            });

            InitializeResult initResult = new InitializeResult
            {
                Capabilities = new ServerCapabilities
                {
                    HoverProvider = true,
                    RenameProvider = true,
                    TextDocumentSync = new TextDocumentSyncOptions
                    {
                        OpenClose = true
                    }
                }
            };

            IRpcHandleResult<InitializeResult> handleResult = new SuccessResult<InitializeResult>(initResult);
            return handleResult;
        }
    }
}