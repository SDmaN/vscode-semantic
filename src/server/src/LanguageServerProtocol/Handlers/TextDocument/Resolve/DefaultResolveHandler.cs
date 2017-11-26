using System.Threading.Tasks;
using JsonRpc.Handlers;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Handlers.TextDocument.Completion;

namespace LanguageServerProtocol.Handlers.TextDocument.Resolve
{
    [RemoteMethodHandler("completionItem/resolve")]
    public abstract class DefaultResolveHandler : RemoteMethodHandler
    {
        public abstract Task<IRpcHandleResult<CompletionItem>> Handle([ObjectParam] CompletionItem item);
    }
}