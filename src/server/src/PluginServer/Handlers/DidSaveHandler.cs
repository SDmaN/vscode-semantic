using System.Threading.Tasks;
using LanguageServerProtocol.Handlers.TextDocument;
using LanguageServerProtocol.Handlers.TextDocument.DidSave;

namespace PluginServer.Handlers
{
    public class DidSaveHandler : DefaultDidSaveHandler
    {
        public override Task Handle(TextDocumentIdentifier textDocument, string text)
        {
            return Task.CompletedTask;
        }
    }
}