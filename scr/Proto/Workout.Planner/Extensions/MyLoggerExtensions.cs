namespace Workout.Planner.Extensions
{
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.Extensions.Logging;

    public static class MyLoggerExtensions
    {
        public static void LoggingError(this ILogger logger, object caller, string message, [CallerMemberName] string methodName = "")
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(caller);
            if (!logger.IsEnabled(LogLevel.Error))
            {
                return;
            }

            logger.LogError($"Class:{caller.GetType().Name} Method:{methodName} Message:{message}");
        }

        public static void LoggingException(this ILogger logger, object caller, Exception exception, [CallerMemberName] string methodName = "")
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(caller);
            if (!logger.IsEnabled(LogLevel.Error))
            {
                return;
            }

            logger.LogCritical(exception, $"Class:{caller.GetType().Name} Method:{methodName}");
        }

        public static void LoggingInformation(this ILogger logger, string message, object caller, [CallerMemberName] string methodName = "")
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(caller);
            if (!logger.IsEnabled(LogLevel.Information))
            {
                return;
            }

            logger.LogInformation(message, $"Class:{caller.GetType().Name} Method:{methodName}");
        }
    }
}
