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
            var root = Directory.GetCurrentDirectory();

            Configuration = new ConfigurationBuilder()
                .SetBasePath(root)
                .AddToucan()
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(Configuration)
                .UseKestrel()
                .UseContentRoot(root)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
