using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Toucan.Contract;
using Toucan.Service;

namespace Toucan.Server.Controllers.Admin
{
    [Authorize(Policy = Policies.ManagerUserAccounts)]
    [Route("api/manage/user/[action]")]
    [ServiceFilter(typeof(Filters.ApiResultFilter))]
    [ServiceFilter(typeof(Filters.ApiExceptionFilter))]
    public class ManageUserController : Controller
    {
        private readonly IManageUserService manageUserService;
        public ManageUserController(IManageUserService manageUserService)
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

            return new
            {
                User = user,
                AvailableRoles = availableRoles
            };
        }

        [HttpGet]
        public async Task<object> Search(int page, int pageSize)
        {
            return await this.manageUserService.Search(page, pageSize);
        }

        [HttpPut]
        public async Task<object> UpdateUser([FromBody]Service.Model.User user)
        {
            return await this.manageUserService.UpdateUser(user);
        }

        [HttpPut]
        public async Task<object> UpdateUserStatus([FromBody] UpdateUserStatusOptions options)
        {
            if (options == null || string.IsNullOrEmpty(options.UserName))
                throw new ServiceException(Constants.FailedToResolveUser);

            return await this.manageUserService.UpdateUserStatus(options.UserName, options.Enabled, options.Verified);
        }

        public class UpdateUserStatusOptions
        {
            public string UserName { get; set; }
            public bool Enabled { get; set; }
            public bool Verified { get; set; }
        }
    }
}
