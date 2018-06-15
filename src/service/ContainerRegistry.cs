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
            For<IExternalAuthenticationProvider>().Add<MicrosoftAuthenticationProvider>();
            
            For<IAuditService>().Use<AuditService>();
            For<IManageUserService>().Use<ManageUserService>();
            For<IManageProfileService>().Use<ManageUserService>();
            For<ISignupService>().Use<SignupService>();
            For<IVerificationProvider>().Use<SmtpVerificationProvider>();
            For<ITokenProviderService<Token>>().Use<TokenProviderService>();
        }
    }
}