namespace JsonRpc.Exceptions
{
    internal class ParameterNotFoundException : ParameterException
    {
        public ParameterNotFoundException(string parameterName, string message)
            : base(parameterName, message)
        {
        }
    }
}