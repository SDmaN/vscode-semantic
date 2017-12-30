using System.Threading.Tasks;
using JsonRpc.Handlers;

namespace LanguageServerProtocol.Handlers.Initialized
{
    [RemoteMethodHandler("initialized")]
    public abstract class DefaultInitilalizedHandler : RemoteMethodHandler
    {
        public abstract Task Handle();
    }
}