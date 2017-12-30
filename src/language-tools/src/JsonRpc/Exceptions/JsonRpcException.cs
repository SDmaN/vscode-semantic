using System;
using System.Runtime.Serialization;

namespace JsonRpc.Exceptions
{
    public class JsonRpcException : ApplicationException
    {
        public JsonRpcException()
        {
        }

        protected JsonRpcException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public JsonRpcException(string message)
            : base(message)
        {
        }

        public JsonRpcException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}