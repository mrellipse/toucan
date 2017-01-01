using System.Security.Claims;
using System.Threading.Tasks;

namespace Toucan.Contract
{
    public interface ILocalAuthenticationService
    {
        Task<ClaimsIdentity> ResolveUser(string email, string password);
        Task<ClaimsIdentity> SignupUser(ISignupOptions options);
    }

    public interface ISignupOptions
    {
        string Email { get; }
        bool Enabled { get; }
        string Name { get; }
        string Password {get;}
        bool Verified { get;}
        string[] Roles { get;}
    }
}
