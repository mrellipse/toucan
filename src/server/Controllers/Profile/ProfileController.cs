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

namespace Toucan.Server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    [ServiceFilter(typeof(Filters.IdentityMappingFilter))]
    public class ProfileController : Controller
    {
        private readonly CultureService cultureService;
        private readonly ILocalizationService localization;
        private readonly IManageProfileService profileService;

        public ILocalizationService localizationService { get; }

        public ProfileController(CultureService cultureService, ILocalizationService localization, IManageProfileService profileService)
        {
            this.cultureService = cultureService;
            this.localization = localization;
            this.profileService = profileService;
        }

        [HttpPut]
        [IgnoreAntiforgeryToken(Order=1000)]
        [Route("[action]")]
        public async Task<object> UpdateUserCulture([FromBody] UpdateUserCultureOptions user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.CultureName))
                throw new ServiceException(Constants.UnsupportedCulture);

            if (!this.localization.IsSupportedCulture(user.CultureName))
                throw new ServiceException(Constants.UnsupportedCulture);

            string cultureName = user.CultureName;
            string timeZoneId = user.TimeZoneId;
            
            if (this.ApplicationUser() != null && this.ApplicationUser().UserId == user.UserId)
            {
                var dbUser = await this.profileService.UpdateUserCulture(user.UserId, user.CultureName, user.TimeZoneId);

                if (dbUser != null)
                {
                    cultureName = dbUser.CultureName;
                    timeZoneId = dbUser.TimeZoneId;
                }
            }

            var resources = await localization.ResolveCulture(cultureName);

            this.cultureService.RefreshCookie(this.HttpContext, user.CultureName, user.TimeZoneId);

            return new
            {
                CultureName = cultureName,
                Resources = resources,
                TimeZoneId = timeZoneId
            };
        }
    }
}
