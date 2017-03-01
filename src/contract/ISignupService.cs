using System.Security.Claims;
using System.Threading.Tasks;

namespace Toucan.Contract
{
    public interface ISignupService
    {
        Task<ClaimsIdentity> SignupUser(ISignupServiceOptions options);
        Task<ClaimsIdentity> RedeemCode(string code, IUser user);
        Task<string> IssueCode(IVerificationProvider provider, IUser user);
    }
}
