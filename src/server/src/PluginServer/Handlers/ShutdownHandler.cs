using System.Threading.Tasks;
using JsonRpc.HandleResult;
using LanguageServerProtocol.Handlers.Shutdown;

namespace PluginServer.Handlers
{
    public class ShutdownHandler : DefaultShutdownHandler
    {
        public override Task<IRpcHandleResult> Handle()
        {
            IRpcHandleResult rpcHandleResult = new SuccessResult(null);
            return Task.FromResult(rpcHandleResult);
        }
    }
}