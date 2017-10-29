using System;

namespace LanguageServerProtocol.Exceptions
{
    public class HeadersParsingException : ApplicationException
    {
        public HeadersParsingException(string line, string message)
            : base(message)
        {
            Line = line;
        }

        public string Line { get; }
    }
}