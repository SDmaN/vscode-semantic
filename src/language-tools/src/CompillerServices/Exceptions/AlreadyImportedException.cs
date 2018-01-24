namespace CompillerServices.Exceptions
{
    public class AlreadyImportedException : CompillerException
    {
        public AlreadyImportedException(string message, string moduleName, int line, int column)
            : base(message, moduleName, line, column)
        {
        }
    }
}