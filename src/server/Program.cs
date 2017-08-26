using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Toucan.Server
{
    public class WebApp
    {
        internal static IConfigurationRoot Configuration;

        public static void Main(string[] args)
        {
            string path = ResolvePath($"app.{WebApp.CurrentEnvironmentName.ToLower()}.json");

            Configuration = new ConfigurationBuilder()
                .SetBasePath(WebApp.BasePath)
                .AddEnvironmentVariables("ASPNETCORE_")
                .AddEnvironmentVariables("TOUCAN_")
                .AddJsonFile(path, optional: false)
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(Configuration)
                .UseKestrel()
                .UseContentRoot(WebApp.BasePath)
                .UseStartup<Startup>()
                .Build();

            host.Run();
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

        private static string environmentName;
        internal static string CurrentEnvironmentName
        {
            get
            {
                if (environmentName == null)
                    environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                return environmentName;
            }
        }

        internal static string ResolvePath(string relativePath)
        {
            return Path.Combine(BasePath, relativePath);
        }

        public static string ResolveApiUrl()
        {
            List<string> urls = new List<string>();

            string value = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

            if (value != null)
                urls.AddRange(value.Split(';'));

            return urls.FirstOrDefault();
        }
    }
}
