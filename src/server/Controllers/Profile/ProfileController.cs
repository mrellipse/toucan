using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Toucan.Contract;
using Toucan.Service;
using Toucan.Server.Model;
using Toucan.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Toucan.Contract.Model;
using Toucan.Contract.Security;

namespace Toucan.Server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    [ServiceFilter(typeof(Filters.IdentityMappingFilter))]
    public class ProfileController : ControllerBase
    {
        private readonly CultureService cultureService;
        private readonly IManageProfileService profileService;
        private readonly ITokenProviderService<Token> tokenService;

        public ILocalizationService localizationService { get; }

        public ProfileController(CultureService cultureService, IManageProfileService profileService, ITokenProviderService<Token> tokenService, IDomainContextResolver resolver, ILocalizationService localization) : base(resolver, localization)
        {
            this.cultureService = cultureService;
            this.profileService = profileService;
            this.tokenService = tokenService;
        }

        [HttpPut]
        [IgnoreAntiforgeryToken(Order = 1000)]
        [Route("[action]")]
        public async Task<object> UpdateUserCulture([FromBody] UpdateUserCultureOptions user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.CultureName))
                this.ThrowLocalizedServiceException(Constants.UnknownUser);

            if (!this.Localization.IsSupportedCulture(user.CultureName))
                this.ThrowLocalizedServiceException(Constants.UnknownCulture);

            string cultureName = user.CultureName;
            string timeZoneId = user.TimeZoneId;
            Token? token = null;

            if (this.ApplicationUser() != null && this.ApplicationUser().UserId == user.UserId)
            {
                var identity = await this.profileService.UpdateUserCulture(user.UserId, user.CultureName, user.TimeZoneId);

                // if (identity != null)
                //     token = await this.tokenService.IssueToken(identity, identity.Name);
            }

            var resources = await this.Localization.ResolveCulture(cultureName);

            this.cultureService.RefreshCookie(this.HttpContext, user.CultureName, user.TimeZoneId);

            return new
            {
                CultureName = cultureName,
                Resources = resources,
                TimeZoneId = timeZoneId,
                Token = token
            };
        }
    }
}
