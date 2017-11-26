using System;

namespace JsonRpc.Handlers
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class CanIgnoreAttribute : Attribute
    {
    }
}