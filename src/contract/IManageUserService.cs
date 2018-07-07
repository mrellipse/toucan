using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Toucan.Contract
{
    public interface IManageUserService
    {
        Task<IDictionary<string,string>> GetAvailableRoles();
        Task<IUserExtended> ResolveUserBy(long userId);
        Task<IUserExtended> ResolveUserBy(string username);
        Task<ISearchResult<IUserExtended>> Search(int page, int pageSize);
        Task<IUserExtended> UpdateUser(IUserExtended user);
        Task<IUserExtended> UpdateUserStatus(string username, bool enabled);
    }
}
