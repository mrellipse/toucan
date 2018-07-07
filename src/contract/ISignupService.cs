using System.Security.Claims;
using System.Threading.Tasks;

namespace Toucan.Contract
{
    public interface ISignupService
    {
        Task<ClaimsIdentity> SignupUser(ISignupServiceOptions options);
        Task<ClaimsIdentity> RedeemVerificationCode(IUser user, string code);
        Task<string> SendVerificationCode(IUser user, string providerKey);
    }
}
