namespace Toucan.Service
{
    public class Config
    {
        public Config()
        {

        }

        public TokenProviderConfig TokenProvider { get; set; }
        public ExternalProviderConfig[] AuthenticationProviders { get; set; }
        public string ClaimsNamespace { get; set; }
    }
}
