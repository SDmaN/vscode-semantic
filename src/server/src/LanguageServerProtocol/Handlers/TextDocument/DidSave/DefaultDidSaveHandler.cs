using System.Threading.Tasks;
using JsonRpc.Handlers;

namespace LanguageServerProtocol.Handlers.TextDocument.DidSave
{
    [RemoteMethodHandler("textDocument/didSave")]
    public abstract class DefaultDidSaveHandler : RemoteMethodHandler
    {
        public abstract Task Handle(TextDocumentIdentifier textDocument, string text);
    }
}