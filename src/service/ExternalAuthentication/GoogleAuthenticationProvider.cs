using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Toucan.Contract;
using Toucan.Data;
using Toucan.Service.Model;

namespace Toucan.Service
{

    public class GoogleAuthenticationProvider : IExternalAuthenticationProvider
    {
        private readonly ExternalProviderConfig config;
        public GoogleAuthenticationProvider(IOptions<Toucan.Service.Config> options)
        {
            this.config = options.Value.AuthenticationProviders.Single(o => string.Equals(o.ProviderId, ProviderTypes.Google, StringComparison.CurrentCultureIgnoreCase));
        }

        public string ClientId
        {
            get
            {
                return this.config.ClientId;
            }
        }

        public string ProfileUri
        {
            get
            {
                return this.config.ProfileUri;
            }
        }

        public string ProviderId
        {
            get
            {
                return this.config.ProviderId;
            }
        }

        public string RevokeUri
        {
            get
            {
                return this.config.RevokeUri;
            }
        }

        public void RevokeToken(string accessToken)
        {
            string requestUri = this.RevokeUri.Replace("{0}", accessToken);

            using (var client = new HttpClient())
            {
                client.PostAsync(requestUri, null);
            }
        }

        public async Task<IExternalTokenData> GetProfileDataFromProvider(string accessToken)
        {
            ExternalTokenData token = null;
            string requestUri = this.ProfileUri.Replace("{0}", accessToken);
            HttpContent requestContent = null;

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(requestUri, requestContent);
                string content = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<ExternalTokenData>(content);
            }

            return token;
        }
    }
}