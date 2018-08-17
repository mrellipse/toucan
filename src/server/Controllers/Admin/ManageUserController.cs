using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Toucan.Contract;
using Toucan.Service;
using Toucan.Server.Model;
using Toucan.Service.Model;
using Toucan.Contract.Security;
using Toucan.Server.Security;
using Toucan.Service.Security;

namespace Toucan.Server.Controllers.Admin
{
    [Authorize(Roles = RoleTypes.Admin + "," + RoleTypes.SiteAdmin)]
    [Route("api/manage/user/[action]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    public class ManageUserController : ControllerBase
    {
        private readonly IManageUserService manageUserService;

        public ManageUserController(IManageUserService manageUserService, IDomainContextResolver resolver, ILocalizationService localization) : base(resolver, localization)
        {
            this.manageUserService = manageUserService;
        }

        [HttpGet]
        public async Task<object> GetUser(string id)
        {
            IUserExtended user = null;
            int userId;

            if (Int32.TryParse(id, out userId))
                user = await this.manageUserService.ResolveUserBy(userId);
            else
                user = await this.manageUserService.ResolveUserBy(id);

            var availableRoles = await this.manageUserService.GetAvailableRoles();
            var availableCultures = await this.Localization.GetSupportedCultures();
            var availableTimeZones = await this.Localization.GetSupportedTimeZones();

            return new
            {
                AvailableRoles = availableRoles,
                AvailableTimeZones = availableTimeZones,
                AvailableCultures = availableCultures,
                User = user,
            };
        }

        [HttpGet]
        public async Task<object> Search(int page, int pageSize)
        {
            return await this.manageUserService.Search(page, pageSize);
        }

        [HttpPut]
        public async Task<object> UpdateUser([FromBody]User user)
        {
            return await this.manageUserService.UpdateUser(user);
        }

        [HttpPut]
        public async Task<object> UpdateUserStatus([FromBody] UpdateUserStatusOptions options)
        {
            if (options == null || string.IsNullOrEmpty(options.UserName))
                this.ThrowLocalizedServiceException(Constants.UnknownUser);

            return await this.manageUserService.UpdateUserStatus(options.UserName, options.Enabled);
        }
    }
}
