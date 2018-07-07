using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Toucan.Contract;
using Toucan.Service;
using Toucan.Server.Model;
using Toucan.Service.Model;

namespace Toucan.Server.Controllers.Admin
{
    [Authorize(Policy = Policies.ManagerUserAccounts)]
    [Route("api/manage/user/[action]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    public class ManageUserController : Controller
    {
        private readonly IManageUserService manageUserService;

        public ILocalizationService localizationService { get; }

        public ManageUserController(IManageUserService manageUserService, ILocalizationService localizationService)
        {
            this.manageUserService = manageUserService;
            this.localizationService = localizationService;
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
            var availableCultures = await this.localizationService.GetSupportedCultures();
            var availableTimeZones = await this.localizationService.GetSupportedTimeZones();

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
                throw new ServiceException(Constants.FailedToResolveUser);

            return await this.manageUserService.UpdateUserStatus(options.UserName, options.Enabled);
        }
    }
}
