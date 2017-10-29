using System;

namespace JsonRpc.Exceptions
{
    public class HandlerNotAnsweredForRequestException : ApplicationException
    {
        public HandlerNotAnsweredForRequestException(string message)
            : base(message)
        {
        }
    }
}