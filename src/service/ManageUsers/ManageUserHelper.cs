using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toucan.Contract;
using Toucan.Data;
using Toucan.Data.Model;

namespace Toucan.Service.Helpers
{
    public class ManageUserHelper
    {
        private readonly User user;
        public ManageUserHelper(User user)
        {
            this.user = user;
        }

        public ManageUserHelper UpdateProfile(IUserExtended user)
        {
            this.user.DisplayName = user.DisplayName;

            return this;
        }

        public ManageUserHelper UpdateStatus(bool enabled, bool verified)
        {
            this.user.Enabled = enabled;
            this.user.Verified = verified;
            
            return this;
        }
    }
}