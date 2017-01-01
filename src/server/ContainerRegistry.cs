using StructureMap;
using Microsoft.Extensions.Configuration;

namespace Toucan.Server
{
    internal class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<IConfiguration>().Use(WebApp.Configuration).Singleton();
        }
    }
}