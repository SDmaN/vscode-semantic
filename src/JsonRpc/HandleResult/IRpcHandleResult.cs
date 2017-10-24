using JsonRpc.Messages;

namespace JsonRpc.HandleResult
{
    public interface IRpcHandleResult
    {
        IResponse GetResponse(MessageId id);
    }
}