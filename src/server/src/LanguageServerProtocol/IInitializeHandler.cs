using System.Threading.Tasks;
using JsonRpc.Handlers;

namespace LanguageServerProtocol
{
    [RemoteMethodHandler("initialize")]
    public abstract class BaseInitializeHandler : RemoteMethodHandler
    {
        public abstract Task Handle(long processId, string rootPath, string rootUri, object capabilities, string trace);
    }
}