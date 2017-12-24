using System;

namespace JsonRpc.Exceptions
{
    public class NotHandlerRegisteringException : ApplicationException
    {
        public NotHandlerRegisteringException(Type registeringType, string message)
            : base(message)
        {
            RegisteringType = registeringType;
        }

        public Type RegisteringType { get; }
    }
}