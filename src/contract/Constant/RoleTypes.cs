using System;
using System.Collections.Generic;

namespace Toucan.Contract.Security
{
    public static class RoleTypes
    {
        public const string Admin = "admin";
        public const string Client = "client";
        public const string SiteAdmin = "siteadmin";
        public const string User = "user";

        public static KeyValuePair<string, string>[] System = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>(RoleTypes.Admin, "Administrator"),
            new KeyValuePair<string, string>(RoleTypes.Client, "Client"),
            new KeyValuePair<string, string>(RoleTypes.SiteAdmin, "Site Administrator"),
            new KeyValuePair<string, string>(RoleTypes.User, "User")
        };
    }
}