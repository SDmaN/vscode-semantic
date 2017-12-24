using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.DidSave;
using LanguageServerProtocol.IPC.Window;

namespace PluginServer.Handlers
{
    public class DidSaveHandler : DefaultDidSaveHandler
    {
        private readonly IWindowMessageSender _messageSender;

        public DidSaveHandler(IWindowMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public override async Task Handle(TextDocumentIdentifier textDocument, string text)
        {
            await _messageSender.LogMessage(MessageType.Info, $"Did save: {textDocument.Uri}");
        }
    }
}