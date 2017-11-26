using System.Threading.Tasks;
using JsonRpc.Handlers;
using JsonRpc.HandleResult;

namespace LanguageServerProtocol.Handlers.TextDocument.Completion
{
    [RemoteMethodHandler("textDocument/completion")]
    public abstract class DefaultCompletionHandler : RemoteMethodHandler
    {
        public abstract Task<IRpcHandleResult<CompletionList>> Handle(TextDocumentIdentifier textDocument,
            Position position, [CanIgnore] CompletionContext context);
    }
}