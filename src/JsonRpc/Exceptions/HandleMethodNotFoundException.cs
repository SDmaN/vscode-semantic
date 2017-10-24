using System;

namespace JsonRpc.Exceptions
{
    public class HandleMethodNotFoundException : ApplicationException
    {
        public HandleMethodNotFoundException(string methodName, Type handlerType, string message)
            : base(message)
        {
            MethodName = methodName;
            HandlerType = handlerType;
        }

        public string MethodName { get; }
        public Type HandlerType { get; }
    }
}