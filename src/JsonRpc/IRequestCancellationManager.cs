using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using JsonRpc.Exceptions;
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

    public class RequestCancellationManager : IRequestCancellationManager
    {
        private readonly IDictionary<MessageId, CancellationTokenSource> _requestCancellations =
            new ConcurrentDictionary<MessageId, CancellationTokenSource>();

        public void AddCancellation(MessageId id)
        {
            if (id == MessageId.Empty)
            {
                return;
            }

            if (_requestCancellations.ContainsKey(id))
            {
                throw new RequestWithIdAlreadyExistsException(id,
                    $"Request with this id ({id}) already exists.");
            }

            CancellationTokenSource requestTokenSource = new CancellationTokenSource();
            _requestCancellations.Add(id, requestTokenSource);
        }

        public void EnsureCancelled(MessageId id)
        {
            if (id == MessageId.Empty)
            {
                return;
            }

            CancellationToken token = _requestCancellations[id].Token;

            if (token.IsCancellationRequested)
            {
                _requestCancellations.Remove(id);
                token.ThrowIfCancellationRequested();
            }
        }

        public void Cancel(MessageId id)
        {
            if (id == MessageId.Empty)
            {
                return;
            }

            _requestCancellations[id].Cancel();
            RemoveCancellation(id);
        }

        public void RemoveCancellation(MessageId id)
        {
            if (id == MessageId.Empty)
            {
                return;
            }

            _requestCancellations.Remove(id);
        }

        public CancellationToken GetToken(MessageId id)
        {
            if (id == MessageId.Empty)
            {
                return default;
            }

            return _requestCancellations[id].Token;
        }
    }
}