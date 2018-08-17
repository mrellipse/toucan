using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Toucan.Contract;
using Toucan.Contract.Security;
using Toucan.Data;
using Toucan.Data.Model;
using Toucan.Service.Helpers;
using Toucan.Service.Model;

namespace Toucan.Service
{
    public class ManageUserService : IManageUserService, IManageProfileService
    {
        private readonly Config config;
        private readonly DbContextBase db;
        private readonly IDeviceProfiler deviceProfiler;

        public ManageUserService(DbContextBase db, IOptions<Config> config, IDeviceProfiler deviceProfiler)
        {
            this.config = config.Value;
            this.db = db;
            this.deviceProfiler = deviceProfiler;
        }

        public async Task<IDictionary<string, string>> GetAvailableRoles()
        {
            var q = (from r in this.db.Role
                     where r.Enabled
                     orderby r.Name ascending
                     select r);

            return await q.AsNoTracking().ToDictionaryAsync(o => o.RoleId, o => o.Name);
        }

        public async Task<IUserExtended> ResolveUserBy(long userId)
        {
            Data.Model.User dbUser = this.db.User.SingleOrDefault(o => o.UserId == userId);

            return await Task.FromResult(this.MapUser(dbUser));
        }

        public async Task<IUserExtended> ResolveUserBy(string username)
        {
            Data.Model.User dbUser = await this.db.User.Where(o => o.Username == username).SingleOrDefaultAsync();

            return this.MapUser(dbUser);
        }

        public Task<ISearchResult<IUserExtended>> Search(int page, int pageSize)
        {
            var q = from u in this.db.User
                .Include(o => o.Roles)
                .Include(o => o.Providers)
                    select u;

            Func<object, Model.User> map = o => MapUser(o as Data.Model.User, false);

            var result = (ISearchResult<IUserExtended>)new SearchResult<IUserExtended>(q.AsNoTracking(), map, page, pageSize);

            return Task.FromResult(result);
        }

        public async Task<IUserExtended> UpdateUser(IUserExtended user)
        {
            Data.Model.User dbUser = this.db.User.SingleOrDefault(o => o.Username == user.Username);

            if (dbUser == null)
                return null;

            new ManageUserHelper(dbUser)
                .UpdateCulture(user)
                .UpdateProfile(user);

            this.db.UpdateUserRoles(dbUser, user.Roles.ToArray());

            await this.db.SaveChangesAsync();

            return this.MapUser(dbUser);
        }
        async Task<IUserExtended> IManageProfileService.UpdateUserCulture(long userId, string cultureName, string timeZoneId)
        {
            Data.Model.User dbUser = this.db.User.SingleOrDefault(o => o.UserId == userId);

            if (dbUser == null)
                return null;

            new ManageUserHelper(dbUser).UpdateCulture(cultureName, timeZoneId);

            await this.db.SaveChangesAsync();

            return this.MapUser(dbUser);
        }

        public async Task<ClaimsIdentity> UpdateUserCulture(long userId, string cultureName, string timeZoneId)
        {
            Data.Model.User dbUser = this.db.User.SingleOrDefault(o => o.UserId == userId);

            if (dbUser == null)
                return null;

            new ManageUserHelper(dbUser).UpdateCulture(cultureName, timeZoneId);

            await this.db.SaveChangesAsync();

            string fingerprint = this.deviceProfiler.DeriveFingerprint(dbUser);
            ClaimsIdentity identity = dbUser.ToClaimsIdentity(this.config.ClaimsNamespace, fingerprint);

            return identity;
        }

        public async Task<IUserExtended> UpdateUserStatus(string username, bool enabled)
        {
            Data.Model.User dbUser = this.db.User.SingleOrDefault(o => o.Username == username);

            if (dbUser == null)
                return null;

            new ManageUserHelper(dbUser).UpdateStatus(enabled);

            await this.db.SaveChangesAsync();

            return this.MapUser(dbUser);
        }

        private Model.User MapUser(Data.Model.User user, bool mapClaims = true)
        {
            if (user == null)
            {
                return new Model.User();
            }

            string[] roles = user.Roles.Select(o => o.RoleId).Distinct().ToArray();
            string[] claims = null;

            if (mapClaims)
                claims = user.Roles.Select(o => o.Role).SelectMany(o => o.SecurityClaims).Select(o => o.SecurityClaimId).Distinct().ToArray();

            return new Model.User()
            {
                CreatedOn = user.CreatedOn,
                CultureName = user.CultureName,
                DisplayName = user.DisplayName,
                Enabled = user.Enabled,
                Claims = claims ?? new string[] { },
                Roles = roles,
                TimeZoneId = user.TimeZoneId,
                UserId = user.UserId,
                Username = user.Username
            };
        }
    }
}