namespace DataModel
{
    using System;

    public class ErrorModel
    {
        public ErrorModel(int statusCode, string? message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public Exception Ec { get; set; }

        public int StatusCode { get; set; }

        public string? Message { get; set; }

        public string? Details { get; set; }


    }
}
