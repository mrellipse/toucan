using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toucan.Contract;
using Toucan.Service;

namespace Toucan.Server.Controllers
{

    [Route("api/[controller]/[action]")]
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
        public async Task<object> Token([FromBody] Model.TokenRequest credentials)
        {
            var identity = await this.authService.ResolveUser(credentials.Username, credentials.password);

            if (identity == null)
                throw new ServiceException(Constants.FailedToResolveUser);

            return await this.tokenService.IssueToken(identity, identity.Name);
        }


        [HttpGet()]
        public async Task<object> ValidateUsername(string username)
        {
            bool available = await this.authService.ValidateUsername(username);

            if (!available)
                throw new ServiceException(Constants.EmailAddressInUse);

            return "Email address is available";
        }

        [HttpPost()]
        public async Task<object> Signup([FromBody]Service.Model.SignupOptions options)
        {
            if (!await this.authService.ValidateUsername(options.Username))
                throw new ServiceException(Constants.EmailAddressInUse);

            options.Roles = new string[] { Toucan.Data.RoleTypes.User };

            var identity = await this.authService.SignupUser(options);

            if (identity == null)
                throw new ServiceException(Constants.FailedToResolveUser);

            return await this.tokenService.IssueToken(identity, identity.Name);
        }

    }
}
