using System;

namespace JsonRpc.Exceptions
{
    public class HandlerException : ApplicationException
    {
        public HandlerException(string message)
            : base(message)
        {
        }
    }
}