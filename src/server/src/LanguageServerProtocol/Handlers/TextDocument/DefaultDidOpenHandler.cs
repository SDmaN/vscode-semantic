using System.Threading.Tasks;
using JsonRpc.Handlers;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    [RemoteMethodHandler("textDocument/didOpen")]
    public abstract class DefaultDidOpenHandler : RemoteMethodHandler
    {
        public abstract Task Handle(TextDocumentItem textDocument);
    }
}