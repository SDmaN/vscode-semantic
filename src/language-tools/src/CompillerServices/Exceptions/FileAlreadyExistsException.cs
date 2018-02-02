using System;

namespace CompillerServices.Exceptions
{
    public class FileAlreadyExistsException : ApplicationException
    {
        public FileAlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}