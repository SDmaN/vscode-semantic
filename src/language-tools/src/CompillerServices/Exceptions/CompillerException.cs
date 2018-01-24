using System;

namespace CompillerServices.Exceptions
{
    public class CompillerException : ApplicationException
    {
        public CompillerException(string message, string moduleName, int line, int column)
            : base(message)
        {
            ModuleName = moduleName;
            Line = line;
            Column = column;
        }

        public string ModuleName { get; }
        public int Line { get; }
        public int Column { get; }
    }
}