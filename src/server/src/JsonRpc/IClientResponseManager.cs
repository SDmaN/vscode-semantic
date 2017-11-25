using System;
using JsonRpc.Messages;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public interface IClientResponseManager
    {
        void RegisterHandler<TResponseResult>(MessageId id, Action<IResponse<TResponseResult>> handler)
            where TResponseResult : class;

        void ResponseIncomming(JToken responseToken);
    }
}