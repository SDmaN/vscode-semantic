using System;

namespace CompillerServices.Exceptions
{
    public class ProjectFileException : ApplicationException
    {
        public ProjectFileException()
        {
        }

        public ProjectFileException(string message)
            : base(message)
        {
        }

        public ProjectFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}