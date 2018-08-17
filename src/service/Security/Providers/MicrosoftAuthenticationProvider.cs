using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Toucan.Contract;
using Toucan.Contract.Security;
using Toucan.Data;
using Toucan.Service.Model;

namespace Toucan.Service
{

    public class MicrosoftAuthenticationProvider : IExternalAuthenticationProvider
    {
        private readonly ExternalProviderConfig config;

        public MicrosoftAuthenticationProvider(IOptions<Toucan.Service.Config> options)
        {
            this.config = options.Value.AuthenticationProviders.Single(o => string.Equals(o.ProviderId, ProviderTypes.Microsoft, StringComparison.CurrentCultureIgnoreCase));
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

        }

        public async Task<IExternalTokenData> GetProfileDataFromProvider(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(accessToken))
                return null;

            var jwtToken = handler.ReadJwtToken(accessToken);

            return await Task.Factory.StartNew(() => MapTo(jwtToken));
        }

        private static IExternalTokenData MapTo(JwtSecurityToken jwtToken)
        {
            int exp;
            Int32.TryParse(jwtToken.Claims.SingleOrDefault(o => o.Type == "exp")?.Value, out exp);

            var token = new ExternalTokenData()
            {
                aud = jwtToken.Audiences.FirstOrDefault(),
                email = jwtToken.Claims.SingleOrDefault(o => o.Type == "email")?.Value,
                email_verified = false,
                exp = exp,
                name = jwtToken.Claims.SingleOrDefault(o => o.Type == "name")?.Value,
                sub = jwtToken.Subject
            };

            return token;
        }
    }
}