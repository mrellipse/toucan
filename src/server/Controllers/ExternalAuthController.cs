using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Toucan.Contract;
using Toucan.Service;
using Toucan.Service.Model;

namespace Toucan.Server.Controllers
{
    [Route("auth/external/[action]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    public class ExternalAuthControllerController : ControllerBase
    {
        private const string IssuedNoncesKey = "IssuedNonces";
        private readonly IExternalAuthenticationService externalAuthService;
        private readonly ITokenProviderService<Token> tokenService;
        private readonly IMemoryCache cache;

        private List<Nonce> IssuedNonces
        {
            get
            {
                return this.cache.GetOrCreate(IssuedNoncesKey, entry => new List<Nonce>());
            }
        }

        public ExternalAuthControllerController(IExternalAuthenticationService externalAuthService, IMemoryCache cache, ITokenProviderService<Token> tokenService, IDomainContextResolver resolver, ILocalizationService localization) : base(resolver, localization)
        {
            this.cache = cache;
            this.externalAuthService = externalAuthService;
            this.tokenService = tokenService;
        }

        [HttpPost()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<object> IssueNonce()
        {
            var nonce = await this.externalAuthService.CreateNonce();

            IssuedNonces.Add(nonce);

            return nonce.Hash;
        }


        [HttpPost()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<object> RedeemToken([FromBody]ExternalLogin options)
        {
            // check for server-generated nonce, and make sure it was issued recently
            if (!IssuedNonces.Any(o => o.Hash == options.Nonce))
                this.ThrowLocalizedServiceException(Constants.InvalidNonce);

            Nonce nonce = IssuedNonces.FirstOrDefault(o => o.Hash == options.Nonce);

            if (nonce.Processing)
                this.ThrowLocalizedServiceException(Constants.InProgressNonce);

            if (nonce.Created.AddMinutes(30) < DateTime.UtcNow)
            {
                IssuedNonces.Remove(nonce);
                this.ThrowLocalizedServiceException(Constants.ExpiredNonce);
            }

            nonce.Update(true);

            // swap the external access token for a local application token
            var identity = await this.externalAuthService.RedeemToken(options);

            if (identity == null)
                this.ThrowLocalizedServiceException(Constants.UnknownUser);

            // remove the original nonce, and revoke the external access token, as they are longer required
            IssuedNonces.Remove(nonce);
            this.externalAuthService.RevokeToken(options.ProviderId, options.AccessToken);

            return await this.tokenService.IssueToken(identity, identity.Name);
        }

        [HttpPost()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<bool> ValidateToken([FromBody]ExternalToken token)
        {
            bool available = await this.externalAuthService.ValidateToken(token.ProviderId, token.AccessToken);

            if (!available)
                this.ThrowLocalizedServiceException(Constants.InvalidAccessToken);

            return true;
        }

    }
}
