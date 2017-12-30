using System.Threading.Tasks;
using JsonRpc.Handlers;

namespace LanguageServerProtocol.Handlers.Exit
{
    [RemoteMethodHandler("exit")]
    public abstract class DefaultExitHandler : RemoteMethodHandler
    {
        public abstract Task Handle();
    }
}