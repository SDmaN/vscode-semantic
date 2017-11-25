using System.Threading.Tasks;
using JsonRpc.Handlers;

namespace LanguageServerProtocol.Handlers.TextDocument.DidClose
{
    [RemoteMethodHandler("textDocument/didClose")]
    public abstract class DefaultDidCloseHandler : RemoteMethodHandler
    {
        public abstract Task Handle(TextDocumentIdentifier textDocument);
    }
}