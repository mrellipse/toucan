using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Toucan.Contract;
using Toucan.Service;

namespace Toucan.Server.Controllers
{
    [Route("auth/[action]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    [ServiceFilter(typeof(Filters.IdentityMappingFilter))]
    public class AuthController : Controller
    {
        private readonly ILocalAuthenticationService authService;
        private readonly ISignupService signupService;
        private readonly ITokenProviderService<Token> tokenService;
        private readonly IVerificationProvider verificationProvider;

        public AuthController(ILocalAuthenticationService authService, ISignupService signupService, IVerificationProvider verificationProvider, ITokenProviderService<Token> tokenService)
        {
            this.authService = authService;
            this.signupService = signupService;
            this.tokenService = tokenService;
            this.verificationProvider = verificationProvider;
        }

        [Authorize]
        [HttpGet()]
        public async Task<object> IssueVerificationCode()
        {
            IUser user = this.ApplicationUser();

            if (user == null)
                throw new ServiceException(Constants.FailedToVerifyUser);

            string code = await this.signupService.IssueCode(this.verificationProvider, user);

            if (code == null)
                throw new ServiceException(Constants.FailedToVerifyUser);

            return code;
        }

        [HttpPost()]
        public async Task<object> Signup([FromBody]Service.Model.LocalSignupOptions options)
        {
            if (!await this.authService.ValidateUser(options.Username))
                throw new ServiceException(Constants.EmailAddressInUse);

            var identity = await this.signupService.SignupUser(options);

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

            return "";
        }

        [Authorize]
        [HttpPut()]
        public async Task<object> RedeemVerificationCode(string code)
        {
            IUser user = this.ApplicationUser();

            if (user == null)
                throw new ServiceException(Constants.FailedToVerifyUser);

            var identity = await this.signupService.RedeemCode(code, user);

            if (identity == null)
                throw new ServiceException(Constants.FailedToVerifyUser);

            return await this.tokenService.IssueToken(identity, identity.Name);
        }
    }
}
