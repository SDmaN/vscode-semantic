using System;

namespace JsonRpc.Exceptions
{
    internal class ParameterNotFoundException : ApplicationException
    {
        public ParameterNotFoundException(string paramName)
        {
            ParamName = paramName;
        }

        public string ParamName { get; }
    }
}