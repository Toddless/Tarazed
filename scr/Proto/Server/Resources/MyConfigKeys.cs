namespace Server.Resources
{
    public class MyConfigKeys
    {
        private string _jWTKey = "Jwt:Key";

        private string _jWTIssuer = "Jwt:Issuer";

        private IConfiguration _config;

        public MyConfigKeys(IConfiguration config)
        {
            _config = config;
        }

        public string JWTKey
        {
            get { return _config[_jWTKey]!; }
        }

        public string JWTIssuer
        {
            get { return _config[_jWTIssuer]!; }
        }
    }
}
