namespace CompillerServices.Exceptions
{
    public class CompillerException : ErrorCheckException
    {
        public CompillerException(string message, string moduleName, int line, int column)
            : base(message, moduleName, line, column)
        {
        }
    }
}