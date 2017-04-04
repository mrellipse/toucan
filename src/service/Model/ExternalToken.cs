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
        public string aud { get; set; }
        public string email { get; set; }
        public bool email_verified { get; set; }
        public int exp { get; set; }
        public string name { get; set; }
        public string sub { get; set; }
    }
}