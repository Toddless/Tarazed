namespace Server.Extensions.Tests
{
    using Microsoft.Extensions.Logging;
    using Moq;

    [TestClass]
    public class LoggerExtensionsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogErrorTest()
        {
            var loggerMock = new Mock<ILogger>(MockBehavior.Loose);
            ILoggerExtensions.LogError(loggerMock.Object, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogWarningTest()
        {
            ILoggerExtensions.LogError(null, null, null);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsEnabledTest(bool isEnabled)
        {
            var loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            loggerMock.Setup(o => o.IsEnabled(LogLevel.Error)).Returns(isEnabled);
            var caller = new object();
            var message = "message";
            //var expectedString = $"Class:{caller.GetType().Name} Method:{nameof(IsEnabledTest)} Message:{message} ";

            if (isEnabled)
            {
                loggerMock.Setup(o => o.Log<It.IsAnyType>(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
            }

            ILoggerExtensions.LogError(loggerMock.Object, caller, message);

            loggerMock.VerifyAll();
        }
    }
}
