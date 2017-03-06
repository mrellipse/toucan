using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Toucan.Server
{
    public class WebApp
    {
        internal static IConfigurationRoot Configuration;
        internal static string ContentRoot;
        public static void Main(string[] args)
        {
            ContentRoot = Directory.GetCurrentDirectory();

            Configuration = new ConfigurationBuilder()
                .SetBasePath(ContentRoot)
                .AddToucan()
                .Build();

            var certPath = Path.Combine(ContentRoot, "Resources\\toucan.dev.pfx");
            var certPassword = "P@ssw0rd";
            var cert = new X509Certificate2(certPath, certPassword);

            var host = new WebHostBuilder()
                .UseConfiguration(Configuration)
                .UseKestrel(options => options.UseHttps(cert))
                .UseContentRoot(ContentRoot)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
