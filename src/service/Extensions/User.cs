using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Toucan.Data;
using Toucan.Data.Model;
using Toucan.Service.Security;
using System.Security.Principal;
using Toucan.Contract.Security;

namespace Toucan.Service
{
    public static partial class Extensions
    {
        public static ClaimsIdentity ToClaimsIdentity(this User user, string ns, string fingerPrint)
        {
            List<Claim> claims = new List<Claim>();

            IEnumerable<Role> roles = MapRoles(user);

            // standard oid claims
            claims.AddRange(MapOpenIdClaims(user, claims));

            // vendor oid claims - used by microsoft
            claims.AddRange(MapRoleClaims(roles));

            // custom authorization claims
            claims.AddRange(MapAuthorizationClaims(roles, ns));

            // custom user profile data
            claims.AddRange(MapProfileClaims(user, ns, fingerPrint, roles));

            var identity = new ClaimsIdentity(new GenericIdentity(user.Username, "Token"), claims.ToArray());

            return identity;
        }

        private static IEnumerable<Role> MapRoles(User user)
        {
            IEnumerable<Role> roles = null;

            if (user.Roles.Any(o => !string.IsNullOrWhiteSpace(o.Role.ParentRoleId)))
            {
                var parents = user.Roles.Select(o => o.Role).Where(o => o.Parent != null).Select(o => o.Parent);
                roles = user.Roles.Select(o => o.Role).Union(parents);
            }
            else
            {
                roles = user.Roles.Select(o => o.Role);
            }

            return roles;
        }

        private static IEnumerable<Claim> MapProfileClaims(User user, string ns, string fingerPrint, IEnumerable<Role> roles)
        {
            bool isVerified = roles.Any(o => o.RoleId == Toucan.Contract.Security.RoleTypes.Admin);  // admin users never have to go through account verification process

            if (!isVerified)
                isVerified = !string.IsNullOrWhiteSpace(fingerPrint) && user.Verifications != null && user.Verifications.Any(o => o.Fingerprint == fingerPrint && o.RedeemedAt != null);

            return new Claim[]
            {
                new Claim(ns + ProfileClaimTypes.CultureName, user.CultureName),
                new Claim(ns + ProfileClaimTypes.TimeZoneId, user.TimeZoneId),
                new Claim(ns + ProfileClaimTypes.Fingerprint, fingerPrint ?? "None"),
                new Claim(ns + ProfileClaimTypes.Verified, isVerified ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())
            };
        }

        private static IEnumerable<Claim> MapOpenIdClaims(User user, List<Claim> claims)
        {
            return new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Username),
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Sid, user.UserId.ToString())
            };
        }

        private static IEnumerable<Claim> MapRoleClaims(IEnumerable<Role> roles)
        {
            return roles.Select(o => new Claim(ClaimTypes.Role, o.RoleId));
        }

        private static IEnumerable<Claim> MapAuthorizationClaims(IEnumerable<Role> roles, string ns)
        {
            var q = from sc in roles.SelectMany(o => o.SecurityClaims)
                    group sc by sc.SecurityClaimId into tmp
                    select new
                    {
                        SecurityClaimId = tmp.Key,
                        DenyClaim = tmp.FirstOrDefault(o => o.Value.Contains(SecurityClaimValueTypes.Deny)),
                        AnyClaim = tmp.FirstOrDefault(o => o.Value.Contains(SecurityClaimValueTypes.Any)),
                        AllClaims = tmp
                    };

            var roleClaims = roles.Select(o => new Claim(ns + o.RoleId, ""));
            var denyClaims = q.Where(o => o.DenyClaim != null).Select(o => o.DenyClaim).Select(o => new Claim(ns + o.SecurityClaimId, o.Value.ToString()));
            var anyClaims = q.Where(o => o.DenyClaim == null && o.AnyClaim != null).Select(o => o.AnyClaim).Select(o => new Claim(ns + o.SecurityClaimId, o.Value.ToString()));

            var otherClaims = from sc in q
                              where sc.DenyClaim == null && sc.AnyClaim == null
                              let value = sc.AllClaims.SelectMany(o => o.Value.ToCharArray()).Distinct().ToArray()
                              select new Claim(ns + sc.SecurityClaimId, new String(value));

            return roleClaims.Union(anyClaims).Union(denyClaims).Union(otherClaims);
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
                        CultureName = principal.TryGetClaimValue<string>(ProfileClaimTypes.CultureName),
                        DisplayName = principal.Identity.Name,
                        Enabled = true,
                        Username = principal.TryGetClaimValue<string>(ClaimTypes.Email),
                        UserId = userId,
                        TimeZoneId = principal.TryGetClaimValue<string>(ProfileClaimTypes.TimeZoneId)
                    };
                }
            }

            return null;
        }

        public static long UserId(this ClaimsPrincipal principal)
        {
            long userId = 0;
            
            if (principal.Identity.IsAuthenticated)
                userId = principal.TryGetClaimValue<long>(ClaimTypes.Sid);

            return userId;
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