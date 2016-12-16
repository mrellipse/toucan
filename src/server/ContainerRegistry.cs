using StructureMap;
using Microsoft.Extensions.Configuration;

namespace Toucan.UI
{
    internal class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<IConfiguration>().Use(WebApp.Configuration).Singleton();
        }
    }
}