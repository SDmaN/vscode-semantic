using System;

namespace JsonRpc.Handlers
{
    public class RemoteMethodHandlerAttribute : Attribute
    {
        public RemoteMethodHandlerAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; }
    }
}