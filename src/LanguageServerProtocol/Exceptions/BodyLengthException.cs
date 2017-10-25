using System;

namespace LanguageServerProtocol.Exceptions
{
    public class BodyLengthException : ApplicationException
    {
        public BodyLengthException(string message)
            : base(message)
        {
        }
    }
}