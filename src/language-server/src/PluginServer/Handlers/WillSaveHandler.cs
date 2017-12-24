using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.WillSave;

namespace PluginServer.Handlers
{
    public class WillSaveHandler : DefaultWillSaveHandler
    {
        public override Task Handle(TextDocumentIdentifier textDocument, TextDocumentSaveReason reason)
        {
            return Task.CompletedTask;
        }
    }
}