using JsonRpc.Messages;

namespace JsonRpc.HandleResult
{
    public interface IRpcHandleResult
    {
        IResponse GetResponse(MessageId id);
    }

    public interface IRpcHandleResult<out TResponseResult> : IRpcHandleResult where TResponseResult : class
    {
        new IResponse<TResponseResult> GetResponse(MessageId id);
    }
}