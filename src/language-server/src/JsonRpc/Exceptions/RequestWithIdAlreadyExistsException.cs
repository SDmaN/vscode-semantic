using JsonRpc.Messages;

namespace JsonRpc.Exceptions
{
    public class RequestWithIdAlreadyExistsException : JsonRpcException
    {
        public RequestWithIdAlreadyExistsException(MessageId id, string message)
            : base(message)
        {
            Id = id;
        }

        public MessageId Id { get; }
    }
}