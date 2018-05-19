using System;
using System.IO;

namespace CompillerServices.Exceptions
{
    public class CompillerException : ApplicationException
    {
        public CompillerException(string message, FileInfo moduleFile, int line, int column)
            : base(message)
        {
            ModuleFile = moduleFile;
            Line = line;
            Column = column;
        }

        public FileInfo ModuleFile { get; }
        public int Line { get; }
        public int Column { get; }
    }
}