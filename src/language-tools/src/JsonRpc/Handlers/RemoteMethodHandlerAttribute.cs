using System;

namespace JsonRpc.Handlers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class RemoteMethodHandlerAttribute : Attribute
    {
        public RemoteMethodHandlerAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; }
    }
}