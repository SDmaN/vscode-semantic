using System;

namespace JsonRpc.Exceptions
{
    public class HandleMethodNotFoundException : HandleMethodException
    {
        public HandleMethodNotFoundException(string methodName, Type handlerType, string message)
            : base(methodName, message)
        {
            HandlerType = handlerType;
        }

        public Type HandlerType { get; }
    }
}