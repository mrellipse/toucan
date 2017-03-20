using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toucan.Contract;
using Toucan.Data;
using Toucan.Data.Model;
using Toucan.Service.Helpers;
using Toucan.Service.Model;

namespace Toucan.Service
{
    public class ManageUserService : IManageUserService
    {
        private readonly DbContextBase db;

        public ManageUserService(DbContextBase db)
        {
            this.db = db;
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
            Data.Model.User dbUser = this.ResolveUser(userId);

            return await Task.FromResult(this.MapUser(dbUser));
        }

        public async Task<IUserExtended> ResolveUserBy(string username)
        {
            Data.Model.User dbUser = this.ResolveUser(username);

            return await Task.FromResult(this.MapUser(dbUser));
        }

        public Task<ISearchResult<IUserExtended>> Search(int page, int pageSize)
        {
            var q = from u in this.db.User.Include(o => o.Roles).Include(o => o.Providers)
                    select u;

            var result = (ISearchResult<IUserExtended>)new SearchResult<IUserExtended>(q.AsNoTracking(), o => MapUser(o as Data.Model.User), page, pageSize);

            return Task.FromResult(result);
        }

        public async Task<IUserExtended> UpdateUser(IUserExtended user)
        {
            Data.Model.User dbUser = this.ResolveUser(user.Username);

            if (dbUser == null)
                return null;

            new ManageUserHelper(dbUser)
                .UpdateProfile(user);

            this.db.UpdateUserRoles(dbUser, user.Roles.ToArray());

            await this.db.SaveChangesAsync();

            return this.MapUser(dbUser);
        }

        public async Task<IUserExtended> UpdateUserStatus(string username, bool enabled, bool verified)
        {
            Data.Model.User dbUser = this.ResolveUser(username);

            if (dbUser == null)
                return null;

            new ManageUserHelper(dbUser).UpdateStatus(enabled, verified);

            await this.db.SaveChangesAsync();

            return this.MapUser(dbUser);
        }

        private Data.Model.User ResolveUser(string username)
        {
            return this.db.User
                .Include(o => o.Roles)
                .Include(o => o.Providers)
                .SingleOrDefault(o => o.Username == username);
        }

        private Data.Model.User ResolveUser(long userId)
        {
            return this.db.User
                .Include(o => o.Roles)
                .Include(o => o.Providers)
                .SingleOrDefault(o => o.UserId == userId);
        }

        private Model.User MapUser(Data.Model.User user)
        {
            if (user == null)
                return new Model.User();
            else
                return new Model.User()
                {
                    DisplayName = user.DisplayName,
                    Enabled = user.Enabled,
                    Roles = user.Roles.Select(o => o.RoleId),
                    UserId = user.UserId,
                    Username = user.Username,
                    Verified = user.Verified
                };
        }
    }
}