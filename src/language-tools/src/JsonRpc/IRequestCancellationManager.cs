using System.Threading;
using JsonRpc.Messages;

namespace JsonRpc
{
    public interface IRequestCancellationManager
    {
        void AddCancellation(MessageId id);
        void EnsureCancelled(MessageId id);
        void Cancel(MessageId id);
        void RemoveCancellation(MessageId id);
        CancellationToken GetToken(MessageId id);
    }
}