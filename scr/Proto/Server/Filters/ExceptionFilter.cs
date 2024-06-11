namespace Server.Filters
{
    using System.Globalization;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var error = new ErrorModel(
                    500,
                    context.Exception.Message,
                    context.Exception.StackTrace?.ToString(CultureInfo.InvariantCulture));
            context.Result = new BadRequestObjectResult(error);
        }
    }
}
