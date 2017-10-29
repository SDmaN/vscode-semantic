using System;
using JsonRpc.Messages;

namespace JsonRpc.Exceptions
{
    public class RequestWithIdAlreadyExistsException : ApplicationException
    {
        public RequestWithIdAlreadyExistsException(MessageId id, string message)
            : base(message)
        {
            Id = id;
        }

        public MessageId Id { get; }
    }
}