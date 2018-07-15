using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Toucan.Contract;
using Toucan.Service;
using Toucan.Service.Security;
using Toucan.Service.Model;

namespace Toucan.Server.Controllers
{
    [Route("auth/[action]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    [ServiceFilter(typeof(Filters.IdentityMappingFilter))]
    public class AuthController : ControllerBase
    {
        private readonly IAntiforgery antiForgeryService;
        private readonly ILocalAuthenticationService authService;
        private readonly CultureService cultureService;
        private readonly Toucan.Server.Config serverConfig;
        private readonly ISignupService signupService;
        private readonly ITokenProviderService<Token> tokenService;

        public AuthController(IAntiforgery antiForgeryService, ILocalAuthenticationService authService, CultureService cultureService, IOptions<Toucan.Server.Config> serverConfig, ISignupService signupService, ITokenProviderService<Token> tokenService, IDomainContextResolver resolver, ILocalizationService localization) : base(resolver, localization)
        {
            this.antiForgeryService = antiForgeryService;
            this.authService = authService;
            this.cultureService = cultureService;
            this.serverConfig = serverConfig.Value;
            this.signupService = signupService;
            this.tokenService = tokenService;
        }

        [Authorize]
        [HttpGet()]
        public async Task<object> IssueVerificationCode(string providerKey = null)
        {
            IUser user = this.ApplicationUser();

            if (user == null)
                this.ThrowLocalizedServiceException(Constants.UnknownUser);

            if (string.IsNullOrWhiteSpace(providerKey))
                providerKey = HttpVerificationProvider.ProviderKey;

            string code = await this.signupService.SendVerificationCode(user, providerKey);

            if (code == null)
                this.ThrowLocalizedServiceException(Constants.FailedToVerifyUser);

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
        public async Task<object> Signup([FromBody]LocalSignupOptions options)
        {
            if (!await this.authService.ValidateUser(options.Username))
                this.ThrowLocalizedServiceException(Constants.EmailAddressInUse);

            var identity = await this.signupService.SignupUser(options);

            if (identity == null)
                this.ThrowLocalizedServiceException(Constants.UnknownUser);

            this.SetAntiforgeryCookies();

            return await this.tokenService.IssueToken(identity, identity.Name);
        }

        [HttpPost()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<object> Token([FromBody] Model.TokenRequest credentials)
        {
            var identity = await this.authService.ResolveUser(credentials.Username, credentials.password);

            if (identity == null)
                this.ThrowLocalizedServiceException(Constants.UnknownUser);

            this.SetAntiforgeryCookies();

            string cultureName = identity.Claims.FirstOrDefault(o => o.Type == CustomClaimTypes.CultureName).Value;
            string timeZoneId = identity.Claims.FirstOrDefault(o => o.Type == CustomClaimTypes.TimeZoneId).Value;

            this.cultureService.RefreshCookie(this.HttpContext, cultureName, timeZoneId);

            return await this.tokenService.IssueToken(identity, identity.Name);
        }

        [HttpGet()]
        [IgnoreAntiforgeryToken(Order = 1000)]
        public async Task<object> ValidateUser(string username)
        {
            bool available = await this.authService.ValidateUser(username);

            if (!available)
                this.ThrowLocalizedServiceException(Constants.EmailAddressInUse);

            return "";
        }

        [Authorize]
        [HttpPut()]
        public async Task<object> RedeemVerificationCode(string code)
        {
            IUser user = this.ApplicationUser();

            if (user == null)
                this.ThrowLocalizedServiceException(Constants.UnknownUser);

            var identity = await this.signupService.RedeemVerificationCode(user, code);

            if (identity == null)
                this.ThrowLocalizedServiceException(Constants.FailedToVerifyUser);

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
