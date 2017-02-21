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
    [Route("auth/[action]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    public class AuthController : Controller
    {
        private readonly ILocalAuthenticationService authService;
        private readonly ITokenProviderService<Token> tokenService;

        public AuthController(ILocalAuthenticationService authService, ITokenProviderService<Token> tokenService)
        {
            this.authService = authService;
            this.tokenService = tokenService;
        }

        [HttpPost()]
        public async Task<object> Signup([FromBody]Service.Model.LocalSignupOptions options)
        {
            if (!await this.authService.ValidateUser(options.Username))
                throw new ServiceException(Constants.EmailAddressInUse);

            var identity = await this.authService.SignupUser(options);

            if (identity == null)
                throw new ServiceException(Constants.FailedToResolveUser);

            return await this.tokenService.IssueToken(identity, identity.Name);
        }

        [HttpPost()]
        public async Task<object> Token([FromBody] Model.TokenRequest credentials)
        {
            var identity = await this.authService.ResolveUser(credentials.Username, credentials.password);

            if (identity == null)
                throw new ServiceException(Constants.FailedToResolveUser);

            return await this.tokenService.IssueToken(identity, identity.Name);
        }

        [HttpGet()]
        public async Task<object> ValidateUser(string username)
        {
            bool available = await this.authService.ValidateUser(username);

            if (!available)
                throw new ServiceException(Constants.EmailAddressInUse);

            return "Email address is available";
        }
    }
}
