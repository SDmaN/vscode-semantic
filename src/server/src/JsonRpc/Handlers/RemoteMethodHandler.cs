using System.Threading;
using JsonRpc.HandleResult;
using JsonRpc.Messages;

namespace JsonRpc.Handlers
{
    public abstract class RemoteMethodHandler
    {
        public IRequest Request { get; internal set; }
        public CancellationToken CancellationToken { get; internal set; }

        protected IRpcHandleResult Ok(object result)
        {
            return new SuccessResult(result);
        }

        protected IRpcHandleResult<TResultValue> Ok<TResultValue>(TResultValue value) where TResultValue : class
        {
            return new SuccessResult<TResultValue>(value);
        }

        protected IRpcHandleResult Error(Error error)
        {
            return new ErrorResult(error);
        }
    }
}