using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Toucan.UI
{
    public class WebApp
    {
        internal static IConfigurationRoot ConfigurationRoot;

        public static void Main(string[] args)
        {
            var contentRoot = Directory.GetCurrentDirectory();

            ConfigurationRoot = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddToucan()
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(ConfigurationRoot)
                .UseKestrel()
                .UseContentRoot(contentRoot)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
