using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Toucan.Data;
using Toucan.Data.Model;
using Toucan.Service.Security;

namespace Toucan.Service
{
    public static partial class Extensions
    {
        public static ClaimsIdentity ToClaimsIdentity(this User user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Username));
            claims.Add(new Claim(ClaimTypes.Name, user.DisplayName));
            claims.Add(new Claim(ClaimTypes.Sid, user.UserId.ToString()));
            claims.Add(new Claim(CustomClaimTypes.Verified, user.Verified ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
            claims.Add(new Claim(CustomClaimTypes.CultureName, user.CultureName));
            claims.Add(new Claim(CustomClaimTypes.TimeZoneId, user.TimeZoneId));

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

        public static User FromClaimsPrincipal(this ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                long userId = principal.TryGetClaimValue<long>(ClaimTypes.Sid);

                if (userId > 0)
                {
                    return new User()
                    {
                        CultureName = principal.TryGetClaimValue<string>(CustomClaimTypes.CultureName),
                        DisplayName = principal.Identity.Name,
                        Enabled = true,
                        Username = principal.TryGetClaimValue<string>(ClaimTypes.Email),
                        UserId = userId,
                        TimeZoneId = principal.TryGetClaimValue<string>(CustomClaimTypes.TimeZoneId),
                        Verified = principal.TryGetClaimValue<bool>(CustomClaimTypes.Verified)
                    };
                }
            }

            return null;
        }

        public static T TryGetClaimValue<T>(this ClaimsPrincipal principal, string type)
        {
            if (!principal.HasClaim(o => o.Type == type))
                return default(T);

            string value = principal.Claims.FirstOrDefault(o => o.Type == type).Value;

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}