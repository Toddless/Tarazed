namespace Server.Extensions
{
    using System.Runtime.CompilerServices;

    public static class ILoggerExtensions
    {
        public static void LogError(this ILogger logger, object caller, string message, [CallerMemberName] string methodName = "")
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(caller);
            if (!logger.IsEnabled(LogLevel.Error))
            {
                return;
            }

            logger.LogError($"Class:{caller.GetType().Name} Method:{methodName} Message:{message} ");
        }

        public static void LogException(this ILogger logger, object caller, Exception exception, [CallerMemberName] string methodName = "")
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(caller);
            if (!logger.IsEnabled(LogLevel.Error))
            {
                return;
            }

            logger.LogCritical(exception, $"Class:{caller.GetType().Name} Method:{methodName}");
        }
    }
}
