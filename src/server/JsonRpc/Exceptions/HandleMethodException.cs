using System;

namespace JsonRpc.Exceptions
{
    public class HandleMethodException : ApplicationException
    {
        public HandleMethodException(string message)
            : this(null, message)
        {
        }

        public HandleMethodException(string methodName, string message)
            : base(message)
        {
            MethodName = methodName;
        }

        public string MethodName { get; }
    }
}