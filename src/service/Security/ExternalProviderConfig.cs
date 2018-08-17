namespace Toucan.Service
{
    public class ExternalProviderConfig
    {
        public ExternalProviderConfig()
        {

        }

        public string ClientId { get; set; }
        public string ProviderId { get; set; }
        public string ProfileUri { get; set; }
        public string RevokeUri { get; set; }
    }
}
