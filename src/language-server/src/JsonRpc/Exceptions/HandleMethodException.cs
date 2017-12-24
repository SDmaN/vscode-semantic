namespace JsonRpc.Exceptions
{
    public class HandleMethodException : JsonRpcException
    {
        public HandleMethodException(string message)
            : this(null, message)
        {
        }

        public HandleMethodException(string methodName, string message)
            : base(message)
        {
            MethodName = methodName;
        }

        public string MethodName { get; }
    }
}