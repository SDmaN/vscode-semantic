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
}