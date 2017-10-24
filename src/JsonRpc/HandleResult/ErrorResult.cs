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
}