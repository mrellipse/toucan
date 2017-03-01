using System;
using System.Linq;
using Toucan.Contract;
using Microsoft.AspNetCore.Mvc;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        internal static string HttpContextCurrentUserKey = "CurrentUser";

        public static IUser ApplicationUser(this ControllerBase controller)
        {
            var user = controller.HttpContext.Items[HttpContextCurrentUserKey];

            return user == null ? null : (IUser)user;
        }
    }
}