using System;
using System.Threading.Tasks;
using JsonRpc.Handlers;
using JsonRpc.HandleResult;

namespace LanguageServerProtocol.Initialize
{
    [RemoteMethodHandler("initialize")]
    public abstract class DefaultInitializeHandler : RemoteMethodHandler
    {
        public abstract Task<IRpcHandleResult<InitializeResult>> Handle(long processId, string rootPath, Uri rootUri,
            ClientCapabilities capabilities, string trace);
    }
}