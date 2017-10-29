namespace JsonRpc.Exceptions
{
    public class HandlerNotFoundException : HandleMethodException
    {
        public HandlerNotFoundException(string methodName, string message)
            : base(methodName, message)
        {
        }
    }
}