using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Configuration;

namespace Toucan.Server
{
    public class WebApp
    {
        internal static IConfigurationRoot Configuration;

        public static void Main(string[] args)
        {
            string[] urls = GetUrlsFromEnv();
            var kestrel = Bootstrap();

            var host = new WebHostBuilder()
                .UseConfiguration(Configuration)
                .UseKestrel(kestrel)
                .UseUrls(urls)
                .UseContentRoot(BasePath)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        private static Action<KestrelServerOptions> Bootstrap()
        {
            string path = ResolvePath($"app.{CurrentEnvironmentName.ToLower()}.json");

            Console.WriteLine($"Loading configuration file {path}");

            Configuration = new ConfigurationBuilder()
                .SetBasePath(BasePath)
                .AddEnvironmentVariables("ASPNETCORE_")
                .AddEnvironmentVariables("TOUCAN_")
                .AddJsonFile(path, optional: false)
                .Build();

            var certificate = Configuration.GetTypedSection<Server.Config.CertificateConfig>("server:certificate");

            return (options) =>
            {
                if (certificate != null && !string.IsNullOrWhiteSpace(certificate.Path))
                {
                    string fileName = ResolvePath(certificate.Path);

                    if (File.Exists(fileName))
                        options.UseHttps(new X509Certificate2(fileName, certificate.Password));
                }
            };
        }

        private static string basePath;
        private static string BasePath
        {
            get
            {
                if (basePath == null)
                {
                    bool isDevelopment = string.Equals(CurrentEnvironmentName, EnvironmentName.Development, StringComparison.CurrentCultureIgnoreCase);

                    if (isDevelopment)
                        basePath = Directory.GetCurrentDirectory();
                    else
                        basePath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;

                    Console.WriteLine($"Base path = {basePath}");
                }

                return basePath;
            }
        }

        private static string currentEnvironmentName;
        internal static string CurrentEnvironmentName
        {
            get
            {
                if (currentEnvironmentName == null)
                    currentEnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                return currentEnvironmentName;
            }
        }

        internal static string ResolvePath(string relativePath)
        {
            return Path.Combine(BasePath, relativePath);
        }
        public static string[] GetUrlsFromEnv()
        {
            List<string> urls = new List<string>();

            string value = Environment.GetEnvironmentVariable("TOUCAN_URLS");

            if (value != null)
                urls.AddRange(value.Split(';'));

            return urls.ToArray();
        }
    }
}
