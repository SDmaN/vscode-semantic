namespace JsonRpc.Exceptions
{
    public class ParameterException : JsonRpcException
    {
        public ParameterException(string message)
            : this(null, message)
        {
        }

        public ParameterException(string parameterName, string message)
            : base(message)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}