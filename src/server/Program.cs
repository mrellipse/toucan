using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Toucan.Server
{
    public class WebApp
    {
        internal static IConfigurationRoot Configuration;

        public static void Main(string[] args)
        {
            var contentRoot = Directory.GetCurrentDirectory();
            
            Configuration = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddToucan()
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(Configuration)
                .UseKestrel()
                .UseContentRoot(contentRoot)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
