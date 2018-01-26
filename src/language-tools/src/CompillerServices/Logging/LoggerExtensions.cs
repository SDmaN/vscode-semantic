using System;
using Microsoft.Extensions.Logging;

namespace CompillerServices.Logging
{
    public static class LoggerExtensions
    {
        public static void LogCompillerTranslates(this ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(CompillerLogEvents.Translate, message, args);
        }

        public static void LogCompillerBuilds(this ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(CompillerLogEvents.Build, message, args);
        }

        public static void LogCompillerCleans(this ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(CompillerLogEvents.Clean, message, args);
        }

        public static void LogCompillerError(this ILogger logger, string message, params object[] args)
        {
            logger.LogError(CompillerLogEvents.Error, message, args);
        }

        public static void LogCompillerError(this ILogger logger, Exception exception)
        {
            logger.LogError(CompillerLogEvents.Error, exception, null);
        }
    }
}