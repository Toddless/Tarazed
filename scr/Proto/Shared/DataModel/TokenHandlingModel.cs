namespace DataModel
{
    /// <summary>
    /// Responsible for token handling, e.g. request to server or response from. Also includes token type and expiration time.
    /// </summary>
    public class TokenHandlingModel
    {
        public string RefreshToken { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";

        public long ExpiresIn { get; set; }
    }
}
