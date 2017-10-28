using System.Collections.Generic;
using System.Threading;
using JsonRpc.Messages;

namespace JsonRpc.Handlers
{
    [RemoteMethodHandler("$/cancelRequest")]
    internal class CancelRequestHandler : RemoteMethodHandler
    {
        public IDictionary<MessageId, CancellationTokenSource> RequestCancellations { get; internal set; }

        public void Handle(MessageId id)
        {
            if (!RequestCancellations.ContainsKey(id))
            {
                return;
            }

            RequestCancellations[id].Cancel();
            RequestCancellations.Remove(id);
        }
    }
}