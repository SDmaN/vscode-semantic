using System;

namespace JsonRpc.Exceptions
{
    public class HandleMethodNotSpecifiedException : HandleMethodException
    {
        public HandleMethodNotSpecifiedException(string message)
            : base(message)
        {
        }
    }
}