namespace Server.Filters
{
    public class ServerException : Exception
    {
        public ServerException(string message, string? rescourceName = null,  Type? resourceType = null)
        : base(message)
        {
            RescourceName = rescourceName ?? message;
            ResourceType = resourceType;
        }

        public string? RescourceName { get; }

        public Type? ResourceType { get; }
    }
}
