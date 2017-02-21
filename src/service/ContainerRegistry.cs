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
            For<IExternalAuthenticationService>().Use<ExternalAuthenticationService>();
            For<ILocalAuthenticationService>().Use<LocalAuthenticationService>();
            For<ITokenProviderService<Token>>().Use<TokenProviderService>();
            For<IExternalAuthenticationProvider>().Add<GoogleAuthenticationProvider>();
        }
    }
}