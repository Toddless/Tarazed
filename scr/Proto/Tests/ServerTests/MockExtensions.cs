namespace ServerTests
{
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;
    using Moq;

    internal static class MockExtensions
    {
        internal static ILogger<TU> SetupLogger<TU>(this object test)
        {
            var mock = new Mock<ILogger<TU>>();
            mock.Setup(m => m.Log<It.IsAnyType>(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
                .Callback<LogLevel, EventId, object, Exception, Delegate>((
                     logLevel,
                     eventId,
                     state,
                     exception,
                     formatter) =>
                {
                    var message = $"Level:{logLevel}{Environment.NewLine}EventId:{eventId}{Environment.NewLine}Message:{formatter.DynamicInvoke(state, exception)}{Environment.NewLine}";
                    Console.WriteLine(message);
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine(message);
                    }
                });
            mock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            return mock.Object;
        }

        internal static ILoggerProvider SetupLoggerProvider<TU>(this object test)
        {
            var mock = new Mock<ILoggerProvider>();
            var mockLogger = test.SetupLogger<TU>();
            mock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(mockLogger);
            return mock.Object;
        }
    }
}
