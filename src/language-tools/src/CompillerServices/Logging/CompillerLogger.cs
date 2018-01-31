using System;
using System.Text;
using CompillerServices.Exceptions;
using Microsoft.Extensions.Logging;

namespace CompillerServices.Logging
{
    internal class CompillerLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception);
            WriteMessage(message, eventId, exception);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return false;

                case LogLevel.Debug:
                    return false;

                case LogLevel.Information:
                    return true;

                case LogLevel.Warning:
                    return false;

                case LogLevel.Error:
                    return true;

                case LogLevel.Critical:
                    return false;

                case LogLevel.None:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        private static void WriteMessage(string message, EventId eventId, Exception exception)
        {
            string prefix = GetPrefix(eventId, exception);
            Console.WriteLine("[{0}]: {1}", prefix, exception?.Message ?? message);
        }

        private static string GetPrefix(EventId eventId, Exception exception)
        {
            StringBuilder prefixBuilder = new StringBuilder(eventId.Name);

            if (!(exception is CompillerException compillerException))
            {
                return prefixBuilder.ToString();
            }

            prefixBuilder.Append($"|{compillerException.ModuleName}");
            prefixBuilder.Append($"|{compillerException.Line}");
            prefixBuilder.Append($":{compillerException.Column}");

            return prefixBuilder.ToString();
        }
    }
}