using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JsonRpc.Messages;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public class ClientResponseManager : IClientResponseManager
    {
        private readonly IDictionary<MessageId, ClientResponseQueueItem> _responseHandlers =
            new ConcurrentDictionary<MessageId, ClientResponseQueueItem>();

        public void RegisterHandler<TResponseResult>(MessageId id, Action<IResponse<TResponseResult>> handler)
            where TResponseResult : class
        {
            _responseHandlers.Add(id, new ClientResponseQueueItem
            {
                Handler = handler,
                HandlerArgumentType = typeof(Response<TResponseResult>)
            });
        }

        public void ResponseIncomming(JToken responseToken)
        {
            MessageId id = responseToken.GetMessageId();

            if (id == MessageId.Empty || !_responseHandlers.TryGetValue(id, out ClientResponseQueueItem item))
            {
                return;
            }

            try
            {
                object handlerArgument = responseToken.ToObject(item.HandlerArgumentType);
                item.Handler.DynamicInvoke(handlerArgument);
            }
            finally
            {
                _responseHandlers.Remove(id);
            }
        }

        private class ClientResponseQueueItem
        {
            public Delegate Handler;
            public Type HandlerArgumentType;
        }
    }
}