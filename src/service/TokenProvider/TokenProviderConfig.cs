namespace Toucan.Service
{
    public class TokenProviderConfig
    {
        public TokenProviderConfig()
        {

        }

        public string TokenSecurityKey { get; set; }
        public string TokenSecurityAlgorithm { get; set; }
        public string TokenIssuer { get; set; }
        public string TokenAudience { get; set; }
    }
}
