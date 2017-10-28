using System.Threading;
using JsonRpc.Messages;

namespace JsonRpc.Handlers
{
    public abstract class RemoteMethodHandler
    {
        public IRequest Request { get; internal set; }
        public CancellationToken CancellationToken { get; internal set; }
    }
}