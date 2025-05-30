﻿namespace Server.Filters
{
    using System.Net;
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
            switch (context.Exception)
            {
                // prozessing expected exceptions with detalized logging
                case ServerException ex:
                    ErrorModel error = new(HttpStatusCode.BadRequest, ex.RescourceName);
                    _logger.LogError(this, $"StatusCode: {error.StatusCode}, Message: {ex.Message}, Details: {ex.StackTrace}");
                    context.Result = new ObjectResult(error);
                    break;

                // default error an client "Something went wrong. Please try again later."
                default:
                    _logger.LogError(this, $"Internal Exception : {context.Exception}");
                    break;
            }
        }
    }
}
