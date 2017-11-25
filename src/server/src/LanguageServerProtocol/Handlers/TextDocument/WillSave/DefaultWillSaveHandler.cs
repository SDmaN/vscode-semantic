using System.Threading.Tasks;
using JsonRpc.Handlers;

namespace LanguageServerProtocol.Handlers.TextDocument.WillSave
{
    [RemoteMethodHandler("textDocument/willSave")]
    public abstract class DefaultWillSaveHandler : RemoteMethodHandler
    {
        public abstract Task Handle(TextDocumentIdentifier textDocument, TextDocumentSaveReason reason);
    }
}