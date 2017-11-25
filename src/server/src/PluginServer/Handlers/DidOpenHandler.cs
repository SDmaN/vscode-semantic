using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.IPC.Window;

namespace PluginServer.Handlers
{
    public class DidOpenHandler : DefaultDidOpenHandler
    {
        private readonly IWindowMessageSender _messageSender;

        public DidOpenHandler(IWindowMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public override async Task Handle(TextDocumentItem textDocument)
        {
            await _messageSender.ShowMessage(new ShowMessageParams
            {
                Message = textDocument.Uri.ToString(),
                Type = MessageType.Info
            });
        }
    }
}