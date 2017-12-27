using System;

namespace CompillerServices.Exceptions
{
    public class ErrorCheckException : ApplicationException
    {
        public ErrorCheckException(string message, int line, int column)
            : base(message)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }
        public int Column { get; }
    }
}