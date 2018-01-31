using Microsoft.Extensions.Logging;

namespace CompillerServices.Logging
{
    internal class CompillerLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CompillerLogger();
        }
    }
}