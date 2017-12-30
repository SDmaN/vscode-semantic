using System.Threading.Tasks;
using JsonRpc.Handlers;

namespace LanguageServerProtocol.Handlers.Workspace.DidChangeConfiguration
{
    [RemoteMethodHandler("workspace/didChangeConfiguration")]
    public abstract class DefaultDidChangeConfigurationHandler : RemoteMethodHandler
    {
        public abstract Task Handle(object settings);
    }
}