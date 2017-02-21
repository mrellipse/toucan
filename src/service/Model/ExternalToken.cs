using Toucan.Contract;

namespace Toucan.Service.Model
{

    public class ExternalToken
    {
        public string AccessToken { get; set; }
        public string ProviderId { get; set; }
    }

    public class ExternalTokenData : IExternalTokenData
    {
        public string access_type { get; set; }
        public string aud { get; set; }
        public string azp { get; set; }
        public string email { get; set; }
        public bool email_verified { get; set; }
        public int exp { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string sub { get; set; }
    }
}