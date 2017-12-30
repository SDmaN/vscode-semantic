using JsonRpc.Messages;

namespace JsonRpc.HandleResult
{
    public class ErrorResult : IRpcHandleResult
    {
        private readonly Error _error;

        public ErrorResult(Error error)
        {
            _error = error;
        }

        public IResponse GetResponse(MessageId id)
        {
            return new Response(id, _error);
        }
    }

    public class ErrorResult<TResponseValue> : ErrorResult, IRpcHandleResult<TResponseValue>
        where TResponseValue : class
    {
        private readonly Error _error;

        public ErrorResult(Error error)
            : base(error)
        {
            _error = error;
        }

        public new IResponse<TResponseValue> GetResponse(MessageId id)
        {
            return new Response<TResponseValue>(id, _error);
        }
    }
}