using System.Threading.Tasks;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Handlers.TextDocument.Completion;
using LanguageServerProtocol.Handlers.TextDocument.Resolve;

namespace PluginServer.Handlers
{
    public class ResolveHandler : DefaultResolveHandler
    {
        public override Task<IRpcHandleResult<CompletionItem>> Handle(CompletionItem item)
        {   
            return Task.FromResult(Ok(item));
        }
    }
}