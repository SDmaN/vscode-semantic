using JsonRpc.Messages;

namespace JsonRpc.HandleResult
{
    public class SuccessResult : IRpcHandleResult
    {
        private readonly object _result;

        public SuccessResult(object result)
        {
            _result = result;
        }

        public IResponse GetResponse(MessageId id)
        {
            return new Response(id, _result);
        }
    }

    public class SuccessResult<TResponseResult> : SuccessResult, IRpcHandleResult<TResponseResult>
        where TResponseResult : class
    {
        private readonly TResponseResult _result;

        public SuccessResult(TResponseResult result)
            : base(result)
        {
            _result = result;
        }

        public new IResponse<TResponseResult> GetResponse(MessageId id)
        {
            return new Response<TResponseResult>(id, _result);
        }
    }
}