using JsonRpc.Messages;

namespace JsonRpc.Handlers
{
    [RemoteMethodHandler("$/cancelRequest")]
    internal class CancelRequestHandler : RemoteMethodHandler
    {
        private readonly IRequestCancellationManager _requestCancellationManager;

        public CancelRequestHandler(IRequestCancellationManager requestCancellationManager)
        {
            _requestCancellationManager = requestCancellationManager;
        }

        public void Handle(MessageId id)
        {
            _requestCancellationManager.Cancel(id);
        }
    }
}