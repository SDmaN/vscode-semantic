namespace JsonRpc.Exceptions
{
    internal class ParametersCountMismatchException : ParameterException
    {
        public ParametersCountMismatchException(string message)
            : base(message)
        {
        }
    }
}