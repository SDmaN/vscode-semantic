using System;

namespace LanguageServerProtocol.Exceptions
{
    public class InvalidHeaderException : ApplicationException
    {
        public InvalidHeaderException(string headerName, string message)
            : base(message)
        {
            HeaderName = headerName;
        }

        public string HeaderName { get; }
    }
}