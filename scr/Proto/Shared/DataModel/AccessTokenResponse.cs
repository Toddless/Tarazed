namespace DataModel
{
    public class AccessTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public long ExpiresIn { get; set; }

        public string RefreshToken { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";
    }
}
