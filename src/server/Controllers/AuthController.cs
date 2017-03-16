using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Toucan.Contract;
using Toucan.Service;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace Toucan.Server.Controllers
{
    [Route("auth/[action]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    [ServiceFilter(typeof(Filters.IdentityMappingFilter))]
    public class AuthController : Controller
    {
        private readonly IAntiforgery antiForgeryService;
        private readonly ILocalAuthenticationService authService;
        private readonly Toucan.Server.Config serverConfig;
        private readonly ISignupService signupService;
        private readonly ITokenProviderService<Token> tokenService;
        private readonly IVerificationProvider verificationProvider;

        public AuthController(IAntiforgery antiForgeryService, ILocalAuthenticationService authService, IOptions<Toucan.Server.Config> serverConfig, ISignupService signupService, IVerificationProvider verificationProvider, ITokenProviderService<Token> tokenService)
        {
            this.antiForgeryService = antiForgeryService;
            this.authService = authService;
            this.serverConfig = serverConfig.Value;
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

        [HttpPut()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<Object> Logout()
        {
            string cookieName = this.serverConfig.AntiForgery.CookieName;

            if (this.HttpContext.Request.Cookies[cookieName] != null)
                this.HttpContext.Response.Cookies.Delete(cookieName);

            string clientName = this.serverConfig.AntiForgery.ClientName;

            if (this.HttpContext.Request.Cookies[clientName] != null)
                this.HttpContext.Response.Cookies.Delete(clientName);

            return await Task.FromResult(true);
        }

        [HttpPost()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<object> Signup([FromBody]Service.Model.LocalSignupOptions options)
        {
            if (!await this.authService.ValidateUser(options.Username))
                throw new ServiceException(Constants.EmailAddressInUse);

            var identity = await this.signupService.SignupUser(options);

            if (identity == null)
                throw new ServiceException(Constants.FailedToResolveUser);

            this.SetAntiforgeryCookies();

            return await this.tokenService.IssueToken(identity, identity.Name);
        }

        [HttpPost()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<object> Token([FromBody] Model.TokenRequest credentials)
        {
            var identity = await this.authService.ResolveUser(credentials.Username, credentials.password);

            if (identity == null)
                throw new ServiceException(Constants.FailedToResolveUser);

            this.SetAntiforgeryCookies();

            return await this.tokenService.IssueToken(identity, identity.Name);
        }

        [HttpGet()]
        [IgnoreAntiforgeryToken(Order = 1000)]
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

        private void ClearAntiforgeryCookies()
        {
            string cookieName = this.serverConfig.AntiForgery.CookieName;

            if (this.HttpContext.Request.Cookies[cookieName] != null)
                this.HttpContext.Response.Cookies.Delete(cookieName);

            string clientName = this.serverConfig.AntiForgery.ClientName;

            if (this.HttpContext.Request.Cookies[clientName] != null)
                this.HttpContext.Response.Cookies.Delete(clientName);
        }

        private void SetAntiforgeryCookies()
        {
            var context = this.HttpContext;
            var tokenSet = antiForgeryService.GetAndStoreTokens(context);

            if (tokenSet.RequestToken != null)
            {
                string clientName = this.serverConfig.AntiForgery.ClientName;
                context.Response.Cookies.Append(clientName, tokenSet.RequestToken, new CookieOptions() { HttpOnly = false, Secure = true });
            }
        }
    }
}
