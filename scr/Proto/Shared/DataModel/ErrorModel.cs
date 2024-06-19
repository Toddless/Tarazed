namespace DataModel
{
    using System.Net;

    public class ErrorModel
    {
        public ErrorModel(HttpStatusCode statusCode, string? message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        private ErrorModel()
        {
        }

        public HttpStatusCode StatusCode { get; set; }

        public string? Message { get; set; }

        public string? Details { get; set; }
    }
}
