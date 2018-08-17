using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toucan.Contract;
using Toucan.Contract.Security;
using Toucan.Data;
using Toucan.Service.Model;
using model = Toucan.Data.Model;

namespace Toucan.Service
{
    public class ManageRoleService : IManageRoleService
    {
        private readonly DbContextBase db;
        public ManageRoleService(DbContextBase db)
        {
            this.db = db;
        }

        public async Task<Role> CloneRole(string roleId)
        {
            Role role = await this.GetRole(roleId);

            if (role != null)
            {
                return role.Clone() as Role;
            }
            else
            {
                return null;
            }
        }

        public async Task<Role> CreateRole(Role role, ClaimsPrincipal currentUser)
        {
            var exists = this.db.Role.Any(o => o.RoleId == role.RoleId);

            if (exists)
                throw new ArgumentException($"A role with unique key '{role.RoleId}' already exists");

            if (string.IsNullOrWhiteSpace(role.ParentRoleId))
                throw new ArgumentException($"A custom role must inherit from a system role");

            if (role.ParentRoleId == RoleTypes.Admin)
                throw new ArgumentException($"You cannot inherit from {RoleTypes.Admin} role");

            var dbRole = new model.Role()
            {
                Enabled = role.Enabled,
                RoleId = role.RoleId,
                Name = role.Name,
                ParentRoleId = role.ParentRoleId,
                CreatedBy = currentUser.UserId(),
                CreatedOn = DateTime.UtcNow
            };

            dbRole.UpdateRoleClaims(role);

            this.db.Role.Add(dbRole);

            await this.db.SaveChangesAsync();

            return dbRole.MapToVm();
        }

        public async Task<IEnumerable<SecurityClaim>> GetAvailableClaims()
        {
            var q = await this.db.SecurityClaim.ToListAsync();

            return q.Select(o => o.MapToVm());
        }

        public async Task<Role> GetRole(string roleId)
        {
            if (!string.IsNullOrWhiteSpace(roleId))
            {
                var q = await this.db.Role.Where(o => o.RoleId == roleId).FirstOrDefaultAsync();
                return q.MapToVm();
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<Role>> GetRoles(params string[] roleIds)
        {
            var q = (from r in this.db.Role
                     where (roleIds.Any() ? roleIds.Contains(r.RoleId) : r.RoleId == r.RoleId)
                     select r);

            return (await q.ToListAsync()).Select(o => o.MapToVm());
        }

        public async Task<IEnumerable<Role>> GetSystemRoles()
        {
            string[] systemRoleIds = RoleTypes.System.Select(o => o.Key).ToArray();

            var q = await this.db.Role.Where(o => systemRoleIds.Contains(o.RoleId)).ToListAsync();

            return q.Select(o => o.MapToVm());
        }

        public Task<ISearchResult<Role>> Search(int page, int pageSize)
        {
            var q = this.db.Role.OrderBy(o => o.Name);

            Func<object, Role> map = o => (o as model.Role).MapToVm();

            var result = (ISearchResult<Role>)new SearchResult<Role>(q, map, page, pageSize);

            return Task.FromResult(result);
        }

        public async Task<Role> UpdateRole(Role role, ClaimsPrincipal currentUser)
        {
            var dbRole = this.db.Role.SingleOrDefault(o => o.RoleId == role.RoleId);

            if (dbRole == null)
                throw new ArgumentNullException($"The role '{role.RoleId}' could not be found");

            if (!string.IsNullOrWhiteSpace(role.ParentRoleId))
            {
                if (role.ParentRoleId == RoleTypes.Admin)
                    throw new ArgumentOutOfRangeException($"You cannot inherit from '{RoleTypes.Admin}' role");

                if (!RoleTypes.System.Any(o => role.ParentRoleId == o.Key))
                    throw new ArgumentOutOfRangeException($"A custom role must inherit from a system role");
            }
            else
            {
                if (!currentUser.IsInRole(RoleTypes.Admin))
                    throw new ServiceException($"You do not have permission to update a system role");

                if (role.RoleId == RoleTypes.Admin)
                    throw new ArgumentOutOfRangeException($"The system role '{RoleTypes.Admin}' cannot be updated");
            }

            dbRole.Enabled = role.Enabled;
            dbRole.Name = role.Name;
            dbRole.LastUpdatedBy = currentUser.UserId();
            dbRole.LastUpdatedOn = DateTime.UtcNow;

            dbRole.UpdateRoleClaims(role);

            await this.db.SaveChangesAsync();

            return dbRole.MapToVm();
        }

        public async Task<Role> UpdateRoleStatus(Role role, ClaimsPrincipal currentUser)
        {
            var dbRole = this.db.Role.SingleOrDefault(o => o.RoleId == role.RoleId);

            if (dbRole == null)
                throw new ArgumentNullException($"The role '{role.RoleId}' could not be found");

            if (dbRole.ParentRoleId == null)
                throw new ArgumentOutOfRangeException($"A system role cannot be enabled or disabled");

            dbRole.Enabled = role.Enabled;
            dbRole.LastUpdatedBy = currentUser.UserId();
            dbRole.LastUpdatedOn = DateTime.UtcNow;

            await this.db.SaveChangesAsync();

            return dbRole.MapToVm();
        }
    }
}