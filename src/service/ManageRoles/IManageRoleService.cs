using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Toucan.Contract;
using Toucan.Service.Model;

namespace Toucan.Service
{
    public interface IManageRoleService
    {
        Task<Role> CloneRole(string roleId);
        Task<Role> CreateRole(Role role, ClaimsPrincipal currentUser);
        Task<IEnumerable<SecurityClaim>> GetAvailableClaims();
        Task<Role> GetRole(string roleId);
        Task<IEnumerable<Role>> GetRoles(params string[] roleIds);
        Task<IEnumerable<Role>> GetSystemRoles();
        Task<ISearchResult<Role>> Search(int page, int pageSize);
        Task<Role> UpdateRole(Role role, ClaimsPrincipal currentUser);
        Task<Role> UpdateRoleStatus(Role role, ClaimsPrincipal currentUser);
    }
}