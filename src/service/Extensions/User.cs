using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Toucan.Data;
using Toucan.Data.Model;

namespace Toucan.Service
{
    public static partial class Extensions
    {
        public static ClaimsIdentity ToClaimsIdentity(this User user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Username));
            claims.Add(new Claim(ClaimTypes.Name, user.DisplayName));
            claims.Add(new Claim("Verified", user.Verified ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));

            var roles = (from r in user.Roles
                         select new Claim(ClaimTypes.Role, r.RoleId)).ToArray();

            if (roles.Length == 0)
                roles = new Claim[] { new Claim(ClaimTypes.Role, RoleTypes.User) };

            claims.AddRange(roles);

            return new ClaimsIdentity(
                new System.Security.Principal.GenericIdentity(user.Username, "Token"),
                claims.ToArray()
                );
        }
    }
}