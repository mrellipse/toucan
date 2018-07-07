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
        public static ClaimsIdentity ToClaimsIdentity(this User user, string fingerPrint = null)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Username));
            claims.Add(new Claim(ClaimTypes.Name, user.DisplayName));
            claims.Add(new Claim(ClaimTypes.Sid, user.UserId.ToString()));
            claims.Add(new Claim(CustomClaimTypes.CultureName, user.CultureName));
            claims.Add(new Claim(CustomClaimTypes.TimeZoneId, user.TimeZoneId));
            claims.Add(new Claim(CustomClaimTypes.Fingerprint, fingerPrint ?? "None"));

            var roles = (from r in user.Roles
                         select new Claim(ClaimTypes.Role, r.RoleId)).ToArray();

            if (roles.Length == 0)
                roles = new Claim[] { new Claim(ClaimTypes.Role, RoleTypes.User) };

            claims.AddRange(roles);

            // admin users never have to go through account verification process
            bool isVerified = user.Roles.Any(o => o.RoleId == RoleTypes.Admin);

            if (!isVerified)
                isVerified = !string.IsNullOrWhiteSpace(fingerPrint) && user.Verifications != null && user.Verifications.Any(o => o.Fingerprint == fingerPrint && o.RedeemedAt != null);

            claims.Add(new Claim(CustomClaimTypes.Verified, isVerified ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));

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
                        TimeZoneId = principal.TryGetClaimValue<string>(CustomClaimTypes.TimeZoneId)
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