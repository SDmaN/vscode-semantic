using System;

namespace JsonRpc.Exceptions
{
    public class InvalidParamsPropertyException : ParameterException
    {
        public InvalidParamsPropertyException(string message)
            : base(message)
        {
        }
    }
}