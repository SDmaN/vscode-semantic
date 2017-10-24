using System;

namespace JsonRpc.Exceptions
{
    public class HandlerNotAnsweredForRequest : ApplicationException
    {
        public HandlerNotAnsweredForRequest(string message)
            : base(message)
        {
        }
    }
}