using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.DidClose;
using LanguageServerProtocol.IPC.Window;

namespace PluginServer.Handlers
{
    public class DidCloseHandler : DefaultDidCloseHandler
    {
        private readonly IWindowMessageSender _messageSender;

        public DidCloseHandler(IWindowMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public override async Task Handle(TextDocumentIdentifier textDocument)
        {
            await _messageSender.LogMessage(MessageType.Info, $"Did close: {textDocument.Uri}");
        }
    }
}