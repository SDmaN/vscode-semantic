namespace CompillerServices.Exceptions
{
    public class KeywordCorrespondingException : CompillerException
    {
        public KeywordCorrespondingException(string message, string moduleName, int line, int column)
            : base(message, moduleName, line, column)
        {
        }
    }
}