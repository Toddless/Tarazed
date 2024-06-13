namespace Server.Filters
{
    using System.Globalization;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Server.Extensions;

    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var error = new ErrorModel(
                    500,
                    context.Exception.Message,
                    context.Exception.StackTrace?.ToString(CultureInfo.InvariantCulture));
            context.Result = new BadRequestObjectResult(error);
            _logger.LogError(this, $"StatusCode: {error.StatusCode}, Message: {error.Message}");
        }
    }
}
