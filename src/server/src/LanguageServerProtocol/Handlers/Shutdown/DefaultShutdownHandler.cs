using System.Threading.Tasks;
using JsonRpc.Handlers;
using JsonRpc.HandleResult;

namespace LanguageServerProtocol.Handlers.Shutdown
{
    [RemoteMethodHandler("shutdown")]
    public abstract class DefaultShutdownHandler : RemoteMethodHandler
    {
        public abstract Task<IRpcHandleResult> Handle();
    }
}