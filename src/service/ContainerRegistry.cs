using StructureMap;
using Toucan.Contract;
using Toucan.Common;

namespace Toucan.Service
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<ICryptoService>().Use<CryptoHelper>();

            For<ILocalAuthenticationService>().Use<LocalAuthenticationService>();
            For<IExternalAuthenticationService>().Use<ExternalAuthenticationService>();
            For<IExternalAuthenticationProvider>().Add<GoogleAuthenticationProvider>();
            For<IManageUserService>().Add<ManageUserService>();
            For<ISignupService>().Use<SignupService>();
            For<IVerificationProvider>().Use<SmtpVerificationProvider>();

            For<ITokenProviderService<Token>>().Use<TokenProviderService>();
        }
    }
}