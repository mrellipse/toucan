using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Toucan.Contract;
using Toucan.Service;
using Toucan.Service.Model;

namespace Toucan.Server.Controllers
{
    [Route("auth/external/[action]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    public class ExternalAuthControllerController : Controller
    {
        private static List<Nonce> IssuedNonces = new List<Nonce>();
        private readonly IExternalAuthenticationService externalAuthService;
        private readonly ITokenProviderService<Token> tokenService;

        public ExternalAuthControllerController(IExternalAuthenticationService externalAuthService, ITokenProviderService<Token> tokenService)
        {
            this.externalAuthService = externalAuthService;
            this.tokenService = tokenService;
        }

        [HttpPost()]
        public async Task<object> IssueNonce()
        {
            var nonce = await this.externalAuthService.CreateNonce();

            IssuedNonces.Add(nonce);

            return nonce.Hash;
        }


        [HttpPost()]
        public async Task<object> RedeemToken([FromBody]Service.Model.ExternalLogin options)
        {
            // check for server-generated nonce, and make sure it was issued recently
            if (!IssuedNonces.Any(o => o.Hash == options.Nonce))
                throw new ServiceException(Constants.InvalidNonce);

            Nonce nonce = IssuedNonces.FirstOrDefault(o => o.Hash == options.Nonce);

            if (nonce.Created.AddMinutes(30) < DateTime.Now)
                throw new ServiceException(Constants.InvalidNonce);

            // swap the external access token for a local application token
            var identity = await this.externalAuthService.RedeemToken(options);

            if (identity == null)
                throw new ServiceException(Constants.FailedToResolveUser);

            // remove the original nonce, and revoke the external access token, as they are longer required
            IssuedNonces.Remove(nonce);
            this.externalAuthService.RevokeToken(options.ProviderId, options.AccessToken);

            return await this.tokenService.IssueToken(identity, identity.Name);
        }

        [HttpPost()]
        public async Task<bool> ValidateToken([FromBody]ExternalToken token)
        {
            bool available = await this.externalAuthService.ValidateToken(token.ProviderId, token.AccessToken);

            if (!available)
                throw new ServiceException(Constants.InvalidAccessToken);

            return true;
        }

    }
}
