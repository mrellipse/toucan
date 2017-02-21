using System.Security.Claims;
using System.Threading.Tasks;

namespace Toucan.Contract
{
    public interface ILocalAuthenticationService
    {
        Task<ClaimsIdentity> ResolveUser(string username, string password);
        Task<ClaimsIdentity> SignupUser(ILocalSignupOptions options);
        Task<bool> ValidateUser(string username);
    }

    public interface ILocalSignupOptions
    {
        string Username { get; }
        bool Enabled { get; }
        string DisplayName { get; }
        string Password {get;}
        bool Verified { get;}
        string[] Roles { get;}
    }
}
