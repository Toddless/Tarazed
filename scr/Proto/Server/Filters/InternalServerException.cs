namespace Server.Filters
{
    public class InternalServerException : Exception
    {
        public InternalServerException(string? message)
            : base(message)
        {
        }
    }
}
