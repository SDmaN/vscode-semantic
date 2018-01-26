using Microsoft.Extensions.Logging;

namespace CompillerServices.Logging
{
    public class CompillerLoggerProvider : ILoggerProvider
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