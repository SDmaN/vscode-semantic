using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRpc.Handlers;
using JsonRpc.HandleResult;

namespace LanguageServerProtocol.Handlers.TextDocument.Completion
{
    [RemoteMethodHandler("textDocument/completion")]
    public abstract class DefaultCompletionHandler : RemoteMethodHandler
    {
        public abstract Task<IRpcHandleResult<IEnumerable<CompletionItem>>> Handle(TextDocumentIdentifier textDocument,
            Position position, [CanIgnore] CompletionContext context);
    }
}